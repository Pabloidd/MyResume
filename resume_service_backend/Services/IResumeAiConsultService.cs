using System.Threading;
using System.Threading.Tasks;
using resume_service_backend.Models;

namespace resume_service_backend.Services
{
    public interface IResumeAiConsultService
    {
        Task<string> GetAdviceAsync(ResumeData resume, string question, CancellationToken cancellationToken = default);
    }
}
