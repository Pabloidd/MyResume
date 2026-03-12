using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
        /// <summary>
    /// Краткая информация о резюме (для списков)
    /// </summary>
    public class ResumeBasic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // pdf_filename
        public string? Email { get; set; }
        public string? Tags { get; set; } // строка с тегами через запятую
    }
}