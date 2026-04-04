using resume_service_backend.Repositories;
using resume_service_backend.Models;
using Microsoft.Extensions.Options;
using resume_service_backend.Options;

namespace resume_service_backend.Repositories
{
        public class ReviewRepository : AbstractRepository, IReviewRepository
    {
        public ReviewRepository(IOptions<MariaDbOptions> options) : base(options) { }

        public async Task<List<Review>> GetAllAsync()
        {
            return await QueryProcAsync<Review>("sp_get_all_reviews");
        }
    }
}