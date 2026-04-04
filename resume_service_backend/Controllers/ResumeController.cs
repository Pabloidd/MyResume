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
            // Валидация
            var error = RequireString(data.Email, "Email");
            if (error != null) return error;

            error = RequireString(data.FirstName, "Имя");
            if (error != null) return error;

            error = RequireString(data.LastName, "Фамилия");
            if (error != null) return error;

            // Проверяем существование пользователя
            var userExists = await _userRepository.ExistsAsync(data.Email);
            if (!userExists)
                return NotFound($"Пользователь с email {data.Email} не найден");

            // Проверяем лимит резюме
            var resumeCount = await _resumeRepository.GetCountByEmailAsync(data.Email);
            if (resumeCount >= MAX_RESUMES_PER_USER)
                return BadRequest($"Достигнут лимит резюме (максимум {MAX_RESUMES_PER_USER})");

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
                    Email = data.Email,
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
        /// DELETE: api/resumes/{id}
        /// Удалить резюме
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResume(int id)
        {
            var error = RequirePositive(id, "ID резюме");
            if (error != null) return error;

            var resume = await _resumeRepository.GetByIdAsync(id);
            if (resume == null)
                return NotFound("Резюме не найдено");

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