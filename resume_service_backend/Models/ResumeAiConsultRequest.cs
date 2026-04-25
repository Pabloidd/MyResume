namespace resume_service_backend.Models
{
    public class ResumeAiConsultRequest
    {
        /// <summary>Роль из auth-сервиса: user | premium | admin</summary>
        public string OwnerRole { get; set; } = "user";

        public string Question { get; set; } = "";

        /// <summary>Текущее состояние полей резюме с фронта (как при генерации PDF).</summary>
        public ResumeData Resume { get; set; } = new();
    }
}
