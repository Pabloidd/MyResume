using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Публичное резюме с тегами (из sp_get_public_resumes_by_tags)
    /// </summary>
    public class PublicResume
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty; // список тегов через запятую
    }
}