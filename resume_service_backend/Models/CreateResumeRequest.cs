using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
        /// <summary>
    /// Создание нового резюме (для sp_create_resume_with_tags)
    /// </summary>
    public class CreateResumeRequest
    {
        public string Email { get; set; } = string.Empty;
        public byte[] PdfData { get; set; } = Array.Empty<byte>();
        public string PdfFilename { get; set; } = string.Empty;
        public string Status { get; set; } = "private";
        public string TagIds { get; set; } = string.Empty;
    }
}