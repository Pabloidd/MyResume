using System.Data;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using resume_service_backend.Models;
using Microsoft.Extensions.Options;
using resume_service_backend.Options;

namespace resume_service_backend.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public class UserRepository : AbstractRepository, IUserRepository
    {
        public UserRepository(IOptions<MariaDbOptions> options) : base(options) { }

        /// <summary>
        /// Проверить существует ли пользователь с таким email
        /// </summary>
        public async Task<bool> ExistsAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            var count = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM users WHERE email = @email",
                new { email }
            );
            return count > 0;
        }

        /// <summary>
        /// Получить пользователя по email
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT email FROM users WHERE email = @email",
                new { email }
            );
        }

        /// <summary>
        /// Создать нового пользователя
        /// </summary>
        public async Task CreateAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "INSERT INTO users (email) VALUES (@email)",
                new { email }
            );
        }
    }
}