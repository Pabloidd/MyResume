using resume_service_backend.Extensions;

namespace resume_service_backend.Services
{
    public static class ApplicationRunner
    {
        public static async Task RunAsync(string[] args)
        {   
            // Если передан флаг --test-pdf, запускаем только тест
            if (args.Contains("--test-pdf"))
            {
                await TestPdfGeneration.Run();
                return;
            }
            
            // Иначе запускаем обычное приложение
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