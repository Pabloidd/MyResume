using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Резюме (PDF хранится в БД)
    /// </summary>
    public class Resume
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "private"; // 'private' или 'public'
        public byte[] PdfData { get; set; } = Array.Empty<byte>();
        public string? PdfFilename { get; set; }
        
        // Навигационное свойство (не хранится в БД, заполняется отдельно)
        public List<Tag>? Tags { get; set; }
    }
}