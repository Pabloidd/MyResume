using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using resume_service_backend.Models;

namespace resume_service_backend.Repositories
{
    public interface ITagRepository
    {
        /// <summary>
        /// sp_get_tags_by_category - теги по категории
        /// </summary>
        Task<List<Tag>> GetByCategoryAsync(string category);
    }
}