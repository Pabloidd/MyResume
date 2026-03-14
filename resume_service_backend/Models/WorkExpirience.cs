using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
     /// <summary>
    /// Опыт работы
    /// </summary>
    public class WorkExperience
    {
        public string Company { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string? StartDate { get; set; } // в формате YYYY-MM
        public string? EndDate { get; set; }    // в формате YYYY-MM или null если current
        public bool Current { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}