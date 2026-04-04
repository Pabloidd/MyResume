using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    public class UpdateResumeRequest
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public byte[]? PdfData { get; set; }
        public string? PdfFilename { get; set; }
        public string? TagIds { get; set; }
    }
}