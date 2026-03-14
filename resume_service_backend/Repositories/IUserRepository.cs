using System.Threading.Tasks;
using resume_service_backend.Models;


namespace resume_service_backend.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Проверить существует ли пользователь с таким email
        /// </summary>
        /// <param name="email">Email пользователя</param>
        /// <returns>true если пользователь существует</returns>
        Task<bool> ExistsAsync(string email);

        /// <summary>
        /// Получить пользователя по email
        /// </summary>
        /// <param name="email">Email пользователя</param>
        /// <returns>Пользователь или null</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        /// <param name="email">Email нового пользователя</param>
        Task CreateAsync(string email);
    }
}