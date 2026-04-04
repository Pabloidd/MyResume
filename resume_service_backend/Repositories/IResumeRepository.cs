using System.Collections.Generic;
using System.Threading.Tasks;
using resume_service_backend.Models;

namespace resume_service_backend.Repositories
{
    public interface IResumeRepository
    {
        // Существующие методы
        Task<List<ResumeBasic>> GetByEmailAsync(string email);
        Task<List<PublicResume>> GetPublicByTagsAsync(string? tagIds = null);
        Task<List<ResumeBasic>> GetAllBasicAsync();
        Task<string> ToggleStatusAsync(int resumeId);
        Task<int> CreateWithTagsAsync(CreateResumeRequest request);
        
        // НОВЫЕ методы
        /// <summary>
        /// Получить полное резюме по ID (с PDF данными)
        /// </summary>
        Task<Resume?> GetByIdAsync(int id);
        
        /// <summary>
        /// Удалить резюме по ID
        /// </summary>
        Task DeleteAsync(int id);
        
        /// <summary>
        /// Обновить существующее резюме
        /// </summary>
        Task UpdateAsync(UpdateResumeRequest request);
        
        /// <summary>
        /// Получить количество резюме пользователя
        /// </summary>
        Task<int> GetCountByEmailAsync(string email);
    }
}