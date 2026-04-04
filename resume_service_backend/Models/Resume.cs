using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
    {
        public class Resume
        {
            public int Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public byte[] PdfData { get; set; } = System.Array.Empty<byte>();
            public string? PdfFilename { get; set; }
            public List<Tag>? Tags { get; set; }
        }
    }