using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace resume_service_backend.Models
{
        /// <summary>
    /// Результат переключения статуса (возвращает новый статус)
    /// </summary>
    public class ToggleStatusResult
    {
        public string Status { get; set; } = string.Empty;
    }
}