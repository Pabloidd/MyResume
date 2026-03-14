using resume_service_backend.Options;
using resume_service_backend.Repositories;
using resume_service_backend.Services;

namespace resume_service_backend.Extensions
{
    ///<summary>
    /// Статический класс для методов расширения ,
    /// предназначенных для регистрации сервисов приложения.
    /// </summary>
    public static class RegistrationManager
    {
        /// <summary>
        /// Метод расширения для регистрации сервисов приложения.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция сервисов.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MariaDbOptions>(configuration.GetSection("MariaDB"));

            //регистрируем сервисы, Scoped — новый экземпляр на каждый HTTP-запрос 
            services.AddScoped<IResumeRepository, ResumeRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            services.AddScoped<IPdfGenerationService, PdfGenerationService>();

            return services;
        }
    }
}