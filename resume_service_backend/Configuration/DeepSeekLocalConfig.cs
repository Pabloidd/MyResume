namespace resume_service_backend.Configuration
{
    /// <summary>
    /// Ключ API «в коде» (перекрывает appsettings.json → DeepSeek:ApiKey).
    /// Для бесплатного режима: Provider = Groq в конфиге и ключ с https://console.groq.com/keys (префикс gsk_).
    /// </summary>
    internal static class DeepSeekLocalConfig
    {
        public const string ApiKey = "";
    }
}
