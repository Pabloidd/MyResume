using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using resume_service_backend.Controllers;
using resume_service_backend.Repositories;

namespace resume_service_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : BaseController
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        /// <summary>
        /// GET: api/tags/category/{category}
        /// Получить теги по категории (sp_get_tags_by_category)
        /// </summary>
        [HttpGet("category/{category}")]
        public Task<ActionResult> GetByCategory(string category)
        {
            var error = RequireString(category, "Категория");
            if (error != null) return Task.FromResult(error);

            return ExecuteAsync(
                () => _tagRepository.GetByCategoryAsync(category),
                "Теги в данной категории не найдены"
            );
        }
    }
}