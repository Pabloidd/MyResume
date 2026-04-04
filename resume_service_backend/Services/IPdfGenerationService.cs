using System.Threading.Tasks;
using resume_service_backend.Models;

namespace resume_service_backend.Services
{
    /// <summary>
    /// Интерфейс сервиса генерации PDF
    /// </summary>
    public interface IPdfGenerationService
    {
        /// <summary>
        /// Генерирует PDF-резюме на основе данных формы
        /// </summary>
        /// <param name="resumeData">Данные резюме из фронтенда</param>
        /// <returns>Байтовый массив PDF-файла</returns>
        Task<byte[]> GenerateResumePdfAsync(ResumeData resumeData);
        
        /// <summary>
        /// Генерирует PDF-резюме и сохраняет во временный файл (для отладки)
        /// </summary>
        /// <param name="resumeData">Данные резюме</param>
        /// <param name="outputPath">Путь для сохранения файла</param>
        /// <returns>Путь к сохранённому файлу</returns>
        Task<string> GenerateAndSaveAsync(ResumeData resumeData, string outputPath);
    }
}