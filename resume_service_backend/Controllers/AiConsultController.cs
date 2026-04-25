using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using resume_service_backend.Models;
using resume_service_backend.Services;

namespace resume_service_backend.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AiConsultController : ControllerBase
    {
        private readonly IResumeAiConsultService _consult;
        private readonly ILogger<AiConsultController> _logger;
        private readonly IHostEnvironment _env;

        public AiConsultController(
            IResumeAiConsultService consult,
            ILogger<AiConsultController> logger,
            IHostEnvironment env)
        {
            _consult = consult;
            _logger = logger;
            _env = env;
        }

        private object AiFailure(string userMessage, Exception? ex = null)
        {
            if (_env.IsDevelopment() && ex != null)
                return new { success = false, error = userMessage, detail = ex.Message };
            return new { success = false, error = userMessage };
        }

        private static string MapDeepSeekClientMessage(string? httpExceptionMessage)
        {
            var m = httpExceptionMessage ?? "";
            if (m.Contains("402", StringComparison.Ordinal) ||
                m.Contains("Insufficient Balance", StringComparison.OrdinalIgnoreCase))
                return "Недостаточно средств на счёте API (часто у DeepSeek). Пополните баланс на platform.deepseek.com либо переключитесь на бесплатный Groq: в appsettings задайте Provider = Groq и ключ с console.groq.com/keys.";
            if (m.Contains("401", StringComparison.Ordinal) ||
                m.Contains("Authentication", StringComparison.OrdinalIgnoreCase) ||
                m.Contains("invalid api key", StringComparison.OrdinalIgnoreCase))
                return "Ключ API отклонён. Для Groq ключ начинается с gsk_, для DeepSeek — с sk-. Проверьте Provider в appsettings и сам ключ.";
            if (m.Contains("429", StringComparison.Ordinal) ||
                m.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
                return "Слишком много запросов к ИИ. Подождите немного и попробуйте снова.";
            return "Сервис временно недоступен";
        }

        /// <summary>
        /// ИИ-консультант по заполнению резюме (Groq или DeepSeek, см. конфиг DeepSeek:Provider). Доступен ролям premium и admin.
        /// </summary>
        [HttpPost("resume-consult")]
        public async Task<ActionResult> ResumeConsult([FromBody] ResumeAiConsultRequest? body, CancellationToken cancellationToken)
        {
            if (body == null || string.IsNullOrWhiteSpace(body.Question))
                return BadRequest(new { error = "Укажите вопрос." });

            var role = (body.OwnerRole ?? "user").Trim().ToLowerInvariant();
            if (role == "standard") role = "user";
            if (role != "premium" && role != "admin")
            {
                return StatusCode(403, new { error = "ИИ-консультант доступен только на тарифах Premium и Admin." });
            }

            if (body.Resume == null)
                return BadRequest(new { error = "Нет данных резюме." });

            try
            {
                var answer = await _consult.GetAdviceAsync(body.Resume, body.Question, cancellationToken);
                return Ok(new { success = true, answer });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "DeepSeek HTTP error");
                return StatusCode(503, AiFailure(MapDeepSeekClientMessage(ex.Message), ex));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "DeepSeek или конфигурация: {Message}", ex.Message);
                var msg = ex.Message.Contains("not configured", StringComparison.OrdinalIgnoreCase)
                    ? "Не задан ключ API для ИИ. Бесплатный вариант: https://console.groq.com/keys → вставьте ключ в DeepSeek:ApiKey и в appsettings укажите \"Provider\": \"Groq\"."
                    : "Сервис временно недоступен";
                return StatusCode(503, AiFailure(msg, ex));
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Таймаут или обрыв соединения с DeepSeek");
                return StatusCode(503, AiFailure("Превышено время ожидания ответа от ИИ. Проверьте сеть и попробуйте снова.", ex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка ИИ-консультанта");
                return StatusCode(503, AiFailure("Сервис временно недоступен", ex));
            }
        }
    }
}
