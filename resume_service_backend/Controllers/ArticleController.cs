using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using resume_service_backend.Controllers;
using resume_service_backend.Repositories;

namespace resume_service_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : BaseController
    {
        private readonly IArticleRepository _articleRepository;

        public ArticlesController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        /// <summary>
        /// GET: api/articles/preview
        /// Получить превью всех статей (sp_get_articles_preview)
        /// </summary>
        [HttpGet("preview")]
        public Task<ActionResult> GetPreviews()
        {
            return ExecuteAsync(
                () => _articleRepository.GetPreviewsAsync(),
                "Статьи не найдены"
            );
        }

        /// <summary>
        /// GET: api/articles/{id}
        /// Получить полную статью по ID (sp_get_article_by_id)
        /// </summary>
        [HttpGet("{id}")]
        public Task<ActionResult> GetById(int id)
        {
            var error = RequirePositive(id, "ID статьи");
            if (error != null) return Task.FromResult(error);

            return ExecuteAsync(
                () => _articleRepository.GetByIdAsync(id),
                "Статья не найдена"
            );
        }
    }
}