using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Ответ при удалении
    /// </summary>
    public class DeleteResponse
    {
        public int ResumeId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}