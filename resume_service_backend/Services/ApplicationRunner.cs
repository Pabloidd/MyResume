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

            // ===== ДОБАВЛЯЕМ CORS =====
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")  // адрес твоего фронта
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddApplicationServices(builder.Configuration);

            var app = builder.Build();

            // ===== ИСПОЛЬЗУЕМ CORS ПЕРЕД МАРШРУТИЗАЦИЕЙ =====
            app.UseCors("AllowFrontend");

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}