using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using resume_service_backend.Controllers;
using resume_service_backend.Repositories;

namespace resume_service_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// GET: api/users/{email}/exists
        /// Проверить существование пользователя
        /// </summary>
        [HttpGet("{email}/exists")]
        public async Task<ActionResult> Exists(string email)
        {
            var error = RequireString(email, "Email");
            if (error != null) return error;

            var exists = await _userRepository.ExistsAsync(email);
            return Ok(new { email, exists });
        }
    }
}