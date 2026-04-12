using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using resume_service_backend.Controllers;
using resume_service_backend.Models;
using resume_service_backend.Repositories;
using resume_service_backend.Services;
using System.IO;

namespace resume_service_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumesController : BaseController
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPdfGenerationService _pdfService;
        private const int MAX_RESUMES_PER_USER = 10;

        public ResumesController(
            IResumeRepository resumeRepository,
            IUserRepository userRepository,
            IPdfGenerationService pdfService)
        {
            _resumeRepository = resumeRepository;
            _userRepository = userRepository;
            _pdfService = pdfService;
        }

        /// <summary>
        /// GET: api/resumes/by-email/{email}
        /// Получить все резюме пользователя (sp_get_resumes_by_email)
        /// </summary>
        [HttpGet("by-email/{email}")]
        public Task<ActionResult> GetByEmail(string email)
        {
            var error = RequireString(email, "Email");
            if (error != null) return Task.FromResult(error);

            return ExecuteAsync(
                () => _resumeRepository.GetByEmailAsync(email),
                "Резюме не найдены"
            );
        }

        /// <summary>
        /// GET: api/resumes/public
        /// Получить публичные резюме по тегам (sp_get_public_resumes_by_tags)
        /// </summary>
        [HttpGet("public")]
        public Task<ActionResult> GetPublic([FromQuery] string? tagIds)
        {
            return ExecuteAsync(
                () => _resumeRepository.GetPublicByTagsAsync(tagIds),
                "Публичные резюме не найдены"
            );
        }

        /// <summary>
        /// GET: api/resumes/basic
        /// Получить ID и названия всех резюме (sp_get_all_resumes_basic)
        /// </summary>
        [HttpGet("basic")]
        public Task<ActionResult> GetAllBasic()
        {
            return ExecuteAsync(
                () => _resumeRepository.GetAllBasicAsync(),
                "Резюме не найдены"
            );
        }

        /// <summary>
        /// GET: api/resumes/{id}
        /// Получить полную информацию о резюме по ID
        /// </summary>
        [HttpGet("{id}")]
        public Task<ActionResult> GetById(int id)
        {
            var error = RequirePositive(id, "ID резюме");
            if (error != null) return Task.FromResult(error);

            return ExecuteAsync(
                () => _resumeRepository.GetByIdAsync(id),
                "Резюме не найдено"
            );
        }

        /// <summary>
        /// PATCH: api/resumes/{id}/toggle-status
        /// Переключить статус резюме (sp_toggle_resume_status)
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult> ToggleStatus(int id)
        {
            var error = RequirePositive(id, "ID резюме");
            if (error != null) return error;

            return await ExecuteAsync(
                async () =>
                {
                    var newStatus = await _resumeRepository.ToggleStatusAsync(id);
                    return new { resumeId = id, status = newStatus };
                },
                "Резюме не найдено"
            );
        }

        /// <summary>
        /// POST: api/resumes/generate
        /// Генерирует PDF-резюме из данных формы и сохраняет в БД
        /// </summary>
        [HttpPost("generate")]
        public async Task<ActionResult> GenerateResume([FromBody] ResumeData data)
        {
            var ownerEmail = string.IsNullOrWhiteSpace(data.OwnerEmail) ? data.Email : data.OwnerEmail;

            // Валидация
            var error = RequireString(ownerEmail, "Email владельца");
            if (error != null) return error;

            error = RequireString(data.FirstName, "Имя");
            if (error != null) return error;

            error = RequireString(data.LastName, "Фамилия");
            if (error != null) return error;

            // Проверяем существование пользователя-владельца в MariaDB (shadow DB для Foreign keys)
            var userExists = await _userRepository.ExistsAsync(ownerEmail);
            if (!userExists)
            {
                // Автоматически синхронизируем (создаем) пользователя в локальной БД, 
                // если его еще там нет, чтобы не падала ошибка 'Cannot add child row (foreign key)'
                await _userRepository.CreateAsync(ownerEmail);
            }

            // Устанавливаем динамический лимит резюме в зависимости от переданной роли пользователя
            int maxResumes = data.OwnerRole.Equals("premium", StringComparison.OrdinalIgnoreCase) || 
                             data.OwnerRole.Equals("admin", StringComparison.OrdinalIgnoreCase) ? 10 : 2;

            // Проверяем лимит резюме
            var resumeCount = await _resumeRepository.GetCountByEmailAsync(ownerEmail);
            if (resumeCount >= maxResumes)
                return BadRequest($"Со статусом '{data.OwnerRole}' достигнут лимит резюме (максимум {maxResumes})");

            try
            {
                // Генерируем PDF
                var pdfBytes = await _pdfService.GenerateResumePdfAsync(data);

                // Формируем имя файла
                var desiredPosition = string.IsNullOrEmpty(data.DesiredPosition)
                    ? "resume"
                    : string.Join("_", data.DesiredPosition.Split(Path.GetInvalidFileNameChars()));

                var filename = $"{desiredPosition}_{DateTime.Now:yyyyMMdd}.pdf";

                // Получаем строку с ID тегов из запроса (если есть)
                var tagIds = data.TagIds ?? "";

                // Логируем для отладки
                Console.WriteLine($"📝 Создание резюме для {data.Email}");
                Console.WriteLine($"🏷️ Полученные TagIds: '{tagIds}'");
                Console.WriteLine($"🎨 Стиль эксперта: {data.ExpertStyle ?? "не выбран"}");

                // Создаём запрос для репозитория
                var request = new CreateResumeRequest
                {
                    Email = ownerEmail,
                    PdfData = pdfBytes,
                    PdfFilename = filename,
                    Status = "private",
                    TagIds = tagIds
                };

                // Сохраняем в БД
                var resumeId = await _resumeRepository.CreateWithTagsAsync(request);

                var response = new CreateResumeResponse
                {
                    ResumeId = resumeId,
                    Filename = filename,
                    Message = "Резюме успешно создано",
                    Size = pdfBytes.Length,
                    PdfBase64 = Convert.ToBase64String(pdfBytes)
                };

                Console.WriteLine($"✅ Резюме создано с ID: {resumeId}");

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                return StatusCode(500, new { error = $"Ошибка при генерации PDF: {ex.Message}" });
            }
        }

        /// <summary>
        /// GET: api/resumes/{id}/download
        /// Скачать PDF-файл резюме
        /// </summary>
        [HttpGet("{id}/download")]
        public async Task<ActionResult> DownloadResume(int id)
        {
            var error = RequirePositive(id, "ID резюме");
            if (error != null) return error;

            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume == null)
                return NotFound("Резюме не найдено");
    
            // ОТЛАДКА: проверяем что данные есть
            if (resume.PdfData == null || resume.PdfData.Length == 0)
                return BadRequest($"PDF пустой. Size: {resume.PdfData?.Length ?? 0}");
    
            return File(resume.PdfData, "application/pdf", resume.PdfFilename);
        }

        /// <summary>
        /// GET: api/resumes/admin-all
        /// Получить все резюме (включая приватные), только для администратора.
        /// Возвращает модель PublicResume, чтобы фронтенду было удобно отображать.
        /// </summary>
        [HttpGet("admin-all")]
        public async Task<ActionResult> GetAdminAll([FromQuery] string? tagIds)
        {
            var result = await _resumeRepository.GetAllByTagsForAdminAsync(tagIds);
            return Ok(result);
        }

        /// <summary>
        /// DELETE: api/resumes/{id}
        /// Удалить резюме (Требует совпадения email или роль=admin)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResume(int id, [FromQuery] string email, [FromQuery] string role = "user")
        {
            if (string.IsNullOrWhiteSpace(email) && role != "admin")
                return Unauthorized("Требуется авторизация для выполнения этого действия.");

            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume == null)
                return NotFound($"Резюме с ID {id} не найдено");

            if (role != "admin" && resume.Email != email)
                return StatusCode(403, "У вас нет прав на удаление этого резюме.");

            await _resumeRepository.DeleteAsync(id);
            
            return Ok(new DeleteResponse
            {
                ResumeId = id,
                Message = "Резюме успешно удалено"
            });
        }

        /// <summary>
        /// PUT: api/resumes/{id}
        /// Обновить существующее резюме
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateResume(int id, [FromBody] UpdateResumeRequest request)
        {
            // Валидация
            var error = RequirePositive(id, "ID резюме");
            if (error != null) return error;

            if (request.Id != id)
                return BadRequest("ID в URL и теле запроса не совпадают");

            // Проверяем существование резюме
            var existingResume = await _resumeRepository.GetByIdAsync(id);
            if (existingResume == null)
                return NotFound("Резюме не найдено");

            return await ExecuteAsync(
                async () =>
                {
                    await _resumeRepository.UpdateAsync(request);
                    return new { resumeId = id, message = "Резюме обновлено" };
                }
            );
        }

        /// <summary>
        /// GET: api/resumes/{email}/count
        /// Получить количество резюме пользователя
        /// </summary>
        [HttpGet("{email}/count")]
        public async Task<ActionResult> GetCountByEmail(string email)
        {
            var error = RequireString(email, "Email");
            if (error != null) return error;

            var count = await _resumeRepository.GetCountByEmailAsync(email);
            return Ok(new { email, count, max = MAX_RESUMES_PER_USER });
        }
    }
}