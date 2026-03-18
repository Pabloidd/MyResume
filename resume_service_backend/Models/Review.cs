using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
    /// <summary>
    /// Отзыв
    /// </summary>
    public class Review
    {
        public int Id { get; set; }

        public string Full_name { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int Rating { get; set; } // 1-5
    }
}