using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using resume_service_backend.Models;

namespace resume_service_backend.Repositories
{
    /// <summary>
    /// Репозиторий для работы с отзывами
    /// </summary>
    /// <summary>
    /// Репозиторий для работы с отзывами
    /// </summary>
    public interface IReviewRepository
    {
        /// <summary>
        /// sp_get_all_reviews - все отзывы
        /// </summary>
        Task<List<Review>> GetAllAsync();
    }
}