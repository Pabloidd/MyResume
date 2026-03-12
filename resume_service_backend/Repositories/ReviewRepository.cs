using resume_service_backend.Repositories;
using resume_service_backend.Models;

namespace resume_service_backend.Repositories
{
        public class ReviewRepository : AbstractRepository, IReviewRepository
    {
        public ReviewRepository(string connectionString) : base(connectionString) { }

        public async Task<List<Review>> GetAllAsync()
        {
            return await QueryProcAsync<Review>("sp_get_all_reviews");
        }
    }
}