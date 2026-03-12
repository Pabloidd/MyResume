using resume_service_backend.Models;


namespace resume_service_backend.Repositories
{
    public class TagRepository : AbstractRepository, ITagRepository
    {
        public TagRepository(string connectionString) : base(connectionString) { }

        public async Task<List<Tag>> GetByCategoryAsync(string category)
        {
            return await QueryProcAsync<Tag>(
                "sp_get_tags_by_category",
                new { p_category = category }
            );
        }
    }
}