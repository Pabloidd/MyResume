using resume_service_backend.Models;


namespace resume_service_backend.Repositories
{
    public class ResumeRepository : AbstractRepository, IResumeRepository
    {
        public ResumeRepository(string connectionString) : base(connectionString) { }

        public async Task<List<ResumeBasic>> GetByEmailAsync(string email)
        {
            return await QueryProcAsync<ResumeBasic>(
                "sp_get_resumes_by_email",
                new { p_email = email }
            );
        }

        public async Task<List<PublicResume>> GetPublicByTagsAsync(string? tagIds = null)
        {
            return await QueryProcAsync<PublicResume>(
                "sp_get_public_resumes_by_tags",
                new { p_tag_ids = tagIds }
            );
        }

        public async Task<List<ResumeBasic>> GetAllBasicAsync()
        {
            return await QueryProcAsync<ResumeBasic>("sp_get_all_resumes_basic");
        }

        public async Task<string> ToggleStatusAsync(int resumeId)
        {
            var result = await QuerySingleProcAsync<dynamic>(
                "sp_toggle_resume_status",
                new { p_resume_id = resumeId }
            );
            
            return result?.status ?? "unknown";
        }

        public async Task<int> CreateWithTagsAsync(CreateResumeRequest request)
        {
            var result = await QuerySingleProcAsync<dynamic>(
                "sp_create_resume_with_tags",
                new
                {
                    p_email = request.Email,
                    p_pdf_data = request.PdfData,
                    p_pdf_filename = request.PdfFilename,
                    p_status = request.Status,
                    p_tag_ids = request.TagIds
                }
            );
            
            return result?.resume_id ?? 0;
        }
    }
}