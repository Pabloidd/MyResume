using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using resume_service_backend.Models;
using Microsoft.Extensions.Options;
using resume_service_backend.Options;


namespace resume_service_backend.Repositories
{
    public class ArticleRepository : AbstractRepository, IArticleRepository
    {
        public ArticleRepository(IOptions<MariaDbOptions> options) : base(options) { }

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