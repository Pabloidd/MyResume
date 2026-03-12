using resume_service_backend.Extensions;

namespace resume_service_backend.Services
{
    public static class ApplicationRunner
    {
        public static async Task RunAsync(string[] args)
        {   
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddApplicationServices(builder.Configuration);

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}