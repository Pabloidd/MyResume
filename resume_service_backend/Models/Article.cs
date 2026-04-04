using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Статья
    /// </summary>
    public class Article
    {
        public int Id { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Preview { get; set; } = string.Empty;
    }
}