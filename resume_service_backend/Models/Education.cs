using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Образование
    /// </summary>
    public class Education
    {
        public string Institution { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string? StartDate { get; set; } // в формате YYYY-MM
        public string? EndDate { get; set; }    // в формате YYYY-MM или null если current
        public bool Current { get; set; }
    }
}