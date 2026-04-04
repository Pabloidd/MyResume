using resume_service_backend.Models;
using Microsoft.Extensions.Options;
using resume_service_backend.Options;


namespace resume_service_backend.Repositories
{
    public class TagRepository : AbstractRepository, ITagRepository
    {
        public TagRepository(IOptions<MariaDbOptions> options) : base(options) { }

        public async Task<List<Tag>> GetByCategoryAsync(string category)
        {
            return await QueryProcAsync<Tag>(
                "sp_get_tags_by_category",
                new { p_category = category }
            );
        }
    }
}