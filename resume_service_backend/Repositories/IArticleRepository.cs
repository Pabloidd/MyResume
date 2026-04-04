using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using resume_service_backend.Models;

namespace resume_service_backend.Repositories
{
        /// <summary>
    /// Репозиторий для работы со статьями
    /// </summary>
        /// <summary>
    /// Репозиторий для работы со статьями
    /// </summary>
    public interface IArticleRepository
    {
        /// <summary>
        /// sp_get_articles_preview - превью всех статей
        /// </summary>
        Task<List<ArticlePreview>> GetPreviewsAsync();

        /// <summary>
        /// sp_get_article_by_id - полная статья по ID
        /// </summary>
        Task<Article?> GetByIdAsync(int id);
    }
}