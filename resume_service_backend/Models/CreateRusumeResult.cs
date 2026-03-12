using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Результат создания резюме (возвращает ID)
    /// </summary>
    public class CreateResumeResult
    {
        public int ResumeId { get; set; }
    }
}