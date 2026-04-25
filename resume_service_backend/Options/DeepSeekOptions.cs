namespace resume_service_backend.Options
{
    /// <summary>OpenAI-совместимый чат: DeepSeek (нужен баланс на platform.deepseek.com) или Groq (щадящий free tier, console.groq.com).</summary>
    public class DeepSeekOptions
    {
        public const string SectionName = "DeepSeek";

        /// <summary>DeepSeek | Groq (регистронезависимо). По умолчанию DeepSeek, чтобы не ломать существующие конфиги.</summary>
        public string Provider { get; set; } = "DeepSeek";

        /// <summary>
        /// База без завершающего /v1/chat/completions — к ней добавляется /v1/chat/completions.
        /// Пусто: для Groq — https://api.groq.com/openai, для DeepSeek — https://api.deepseek.com
        /// </summary>
        public string ApiBaseUrl { get; set; } = "";

        /// <summary>Пусто: для Groq — llama-3.3-70b-versatile, для DeepSeek — deepseek-v4-flash</summary>
        public string Model { get; set; } = "";

        /// <summary>Ключ API (Groq: gsk_..., DeepSeek: sk-...).</summary>
        public string ApiKey { get; set; } = "";
    }
}
