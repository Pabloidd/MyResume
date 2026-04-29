using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using resume_service_backend.Configuration;
using resume_service_backend.Models;
using resume_service_backend.Options;

namespace resume_service_backend.Services
{
    public class ResumeAiConsultService : IResumeAiConsultService
    {
        private readonly HttpClient _http;
        private readonly DeepSeekOptions _options;
        private readonly ILogger<ResumeAiConsultService> _logger;

        private static readonly JsonSerializerOptions ResumeJsonWrite = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>OpenAI-совместимый JSON: max_tokens, а не maxTokens.</summary>
        private static readonly JsonSerializerOptions DeepSeekRequestJson = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public ResumeAiConsultService(
            HttpClient http,
            IOptions<DeepSeekOptions> options,
            ILogger<ResumeAiConsultService> logger)
        {
            _http = http;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<string> GetAdviceAsync(ResumeData resume, string question, CancellationToken cancellationToken = default)
        {
            var apiKey = !string.IsNullOrWhiteSpace(DeepSeekLocalConfig.ApiKey)
                ? DeepSeekLocalConfig.ApiKey.Trim()
                : (_options.ApiKey ?? "").Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Chat API key is not configured.");

            var apiBase = ResolveApiBase(_options);
            var url = BuildChatCompletionsUrl(apiBase);
            var model = ResolveModel(_options);

            var resumeJson = JsonSerializer.Serialize(resume, ResumeJsonWrite);

            const string systemPrompt =
                "Ты опытный карьерный консультант и редактор резюме. Отвечай по-русски, структурировано и по делу. " +
                "Пользователь заполняет конструктор резюме и опирается на переданный ниже JSON с уже введёнными полями, чтобы составить итоговое резюме. " +
                "Твоя задача — помочь ему в этом: улучшить формулировки, структуру секций, подачу опыта и ответить на вопрос строго в контексте этих данных. " +
                "Дай практические советы по формулировкам, структуре и содержанию. Не выдумывай факты о кандидате — опирайся только на JSON. " +
                "Если данных мало, укажи, чего не хватает. Можно короткие абзацы и маркированные списки.";

            var userContent =
                "Контекст: пользователь использует данные из следующего JSON, чтобы составить резюме в конструкторе, и обращается к тебе за помощью.\n\n" +
                "JSON с текущими полями (имя, контакты, о себе, опыт, образование, навыки и т.д.):\n\n" +
                resumeJson +
                "\n\nВопрос пользователя:\n" + question.Trim();

            var request = new ChatCompletionRequest
            {
                Model = model,
                Temperature = 0.6,
                MaxTokens = 2048,
                Messages =
                [
                    new ChatMessage { Role = "system", Content = systemPrompt },
                    new ChatMessage { Role = "user", Content = userContent }
                ]
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            req.Headers.TryAddWithoutValidation("User-Agent", "MyResumeResumeService/1.0");
            req.Content = JsonContent.Create(request, mediaType: null, DeepSeekRequestJson);

            using var response = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            var rawPreview = string.IsNullOrEmpty(raw) ? "(пусто)" : raw[..Math.Min(800, raw.Length)];

            if (!response.IsSuccessStatusCode)
            {
                var apiErr = TryExtractApiErrorMessage(raw);
                _logger.LogWarning(
                    "Chat API HTTP {Status}: {ApiError}. Тело: {BodyPreview}",
                    (int)response.StatusCode,
                    apiErr ?? "(без поля error.message)",
                    rawPreview);
                var human = apiErr != null
                    ? $"HTTP {(int)response.StatusCode}: {apiErr}"
                    : $"HTTP {(int)response.StatusCode}";
                throw new HttpRequestException(human);
            }

            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("ИИ-провайдер вернул пустой ответ.");

            JsonDocument doc;
            try
            {
                doc = JsonDocument.Parse(raw);
            }
            catch (JsonException jx)
            {
                _logger.LogWarning(jx, "Chat API: не JSON. Превью: {Preview}", rawPreview);
                throw new InvalidOperationException("ИИ-провайдер вернул неожиданный формат ответа.");
            }

            using (doc)
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("error", out var errRoot))
                {
                    var msg = TryExtractErrorMessage(errRoot) ?? "ошибка API";
                    _logger.LogWarning("Chat API ответ 2xx с error: {Msg}. Превью: {Preview}", msg, rawPreview);
                    throw new HttpRequestException(msg);
                }

                if (!root.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                {
                    _logger.LogWarning("Chat API: нет choices. Превью: {Preview}", rawPreview);
                    throw new InvalidOperationException("Unexpected chat API response shape.");
                }

                var first = choices[0];
                if (!first.TryGetProperty("message", out var message) ||
                    !message.TryGetProperty("content", out var contentEl))
                {
                    _logger.LogWarning("Chat API: нет message.content. Превью: {Preview}", rawPreview);
                    throw new InvalidOperationException("Unexpected chat API response shape.");
                }

                var text = contentEl.GetString();
                return string.IsNullOrWhiteSpace(text) ? "Пустой ответ модели." : text.Trim();
            }
        }

        private static bool IsGroqProvider(string? provider) =>
            string.Equals(provider?.Trim(), "Groq", StringComparison.OrdinalIgnoreCase);

        private static string ResolveApiBase(DeepSeekOptions o)
        {
            if (!string.IsNullOrWhiteSpace(o.ApiBaseUrl))
                return o.ApiBaseUrl.Trim().TrimEnd('/');
            return IsGroqProvider(o.Provider)
                ? "https://api.groq.com/openai"
                : "https://api.deepseek.com";
        }

        private static string ResolveModel(DeepSeekOptions o)
        {
            if (!string.IsNullOrWhiteSpace(o.Model))
                return o.Model.Trim();
            return IsGroqProvider(o.Provider)
                ? "llama-3.3-70b-versatile"
                : "deepseek-v4-flash";
        }

        private static string BuildChatCompletionsUrl(string apiBase)
        {
            apiBase = apiBase.Trim().TrimEnd('/');
            if (apiBase.EndsWith("/v1/chat/completions", StringComparison.OrdinalIgnoreCase))
                return apiBase;
            return $"{apiBase}/v1/chat/completions";
        }

        private static string? TryExtractApiErrorMessage(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                using var d = JsonDocument.Parse(json);
                return d.RootElement.TryGetProperty("error", out var e) ? TryExtractErrorMessage(e) : null;
            }
            catch
            {
                return null;
            }
        }

        private static string? TryExtractErrorMessage(JsonElement err)
        {
            if (err.ValueKind == JsonValueKind.String)
                return err.GetString();
            if (err.ValueKind == JsonValueKind.Object && err.TryGetProperty("message", out var m))
                return m.GetString();
            return null;
        }

        private sealed class ChatCompletionRequest
        {
            public string Model { get; set; } = "";
            public List<ChatMessage> Messages { get; set; } = [];
            public double Temperature { get; set; }
            public int MaxTokens { get; set; }
        }

        private sealed class ChatMessage
        {
            public string Role { get; set; } = "";
            public string Content { get; set; } = "";
        }
    }
}
