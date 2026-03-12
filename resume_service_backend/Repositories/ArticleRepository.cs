using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using resume_service_backend.Models;


namespace resume_service_backend.Repositories
{
    public class ArticleRepository : AbstractRepository, IArticleRepository
    {
        public ArticleRepository(string connectionString) : base(connectionString) { }

        public async Task<List<ArticlePreview>> GetPreviewsAsync()
        {
            return await QueryProcAsync<ArticlePreview>("sp_get_articles_preview");
        }

        public async Task<Article?> GetByIdAsync(int id)
        {
            return await QuerySingleProcAsync<Article>(
                "sp_get_article_by_id",
                new { p_article_id = id }
            );
        }
    }
}