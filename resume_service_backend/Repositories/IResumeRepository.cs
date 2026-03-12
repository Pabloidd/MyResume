using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using resume_service_backend.Models;

namespace resume_service_backend.Repositories
{
        public interface IResumeRepository
    {
        /// <summary>
        /// sp_get_resumes_by_email - получить все резюме пользователя
        /// </summary>
        Task<List<ResumeBasic>> GetByEmailAsync(string email);

        /// <summary>
        /// sp_get_public_resumes_by_tags - публичные резюме по тегам
        /// </summary>
        Task<List<PublicResume>> GetPublicByTagsAsync(string? tagIds = null);

        /// <summary>
        /// sp_get_all_resumes_basic - все резюме (ID + название)
        /// </summary>
        Task<List<ResumeBasic>> GetAllBasicAsync();

        /// <summary>
        /// sp_toggle_resume_status - переключить статус
        /// </summary>
        Task<string> ToggleStatusAsync(int resumeId);

        /// <summary>
        /// sp_create_resume_with_tags - создать резюме с тегами
        /// </summary>
        Task<int> CreateWithTagsAsync(CreateResumeRequest request);
    }
}