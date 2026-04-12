using System;
using System.Collections.Generic;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Основная модель данных резюме из фронтенда
    /// </summary>
    public class ResumeData
    {
        // Email владельца аккаунта
        public string? OwnerEmail { get; set; }

        // Роль владельца (выступает основой для лимитов)
        public string OwnerRole { get; set; } = "user";

        // Основная информация
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DesiredPosition { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        
        // Стиль (пока не используется, но оставим)
        public string? ExpertStyle { get; set; }
        
        // Опыт работы
        public List<WorkExperience> WorkExperience { get; set; } = new();
        
        // Образование
        public List<Education> Education { get; set; } = new();
        
        // Навыки
        public List<string> Skills { get; set; } = new();

        /// Строка с ID тегов через запятую (для хранимой процедуры)
        public string? TagIds { get; set; }

        // Метаданные
        public DateTime CreatedAt { get; set; }
        public string Template { get; set; } = "modern";
    }

   

    
}