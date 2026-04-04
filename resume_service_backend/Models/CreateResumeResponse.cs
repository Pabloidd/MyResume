using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
        /// <summary>
    /// Ответ при создании резюме
    /// </summary>
    public class CreateResumeResponse
    {
        public int ResumeId { get; set; }
        public string Filename { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Size { get; set; }
        public string? PdfBase64 { get; set; }
    }
}