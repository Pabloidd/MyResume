using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using resume_service_backend.Controllers;
using resume_service_backend.Repositories;

namespace resume_service_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : BaseController
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewsController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        /// <summary>
        /// GET: api/reviews
        /// Получить все отзывы (sp_get_all_reviews)
        /// </summary>
        [HttpGet]
        public Task<ActionResult> GetAll()
        {
            return ExecuteAsync(
                () => _reviewRepository.GetAllAsync(),
                "Отзывы не найдены"
            );
        }
    }
}