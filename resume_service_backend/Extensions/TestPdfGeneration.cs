using System;
using System.IO;
using System.Threading.Tasks;
using resume_service_backend.Models;
using resume_service_backend.Services;

namespace resume_service_backend
{
    public static class TestPdfGeneration
    {
        public static async Task Run()
        {
            Console.WriteLine("🚀 Тест генерации PDF-резюме");
            Console.WriteLine("==============================");

            // Создаём тестовые данные
            var testData = new ResumeData
            {
                FirstName = "Иван",
                LastName = "Петров",
                DesiredPosition = "Senior .NET Developer",
                Email = "ivan.petrov@example.com",
                Phone = "+7 (999) 123-45-67",
                About = "Опытный разработчик с 8-летним стажем. Специализируюсь на backend-разработке, микросервисах и облачных технологиях. Люблю чистый код и командную работу.",
                CreatedAt = DateTime.Now,
                ExpertStyle = "modern",
                
                // Опыт работы
                WorkExperience = new System.Collections.Generic.List<WorkExperience>
                {
                    new WorkExperience
                    {
                        Company = "ООО Технологии Будущего",
                        Position = "Senior .NET Developer",
                        StartDate = "2021-03",
                        EndDate = null,
                        Current = true,
                        Description = "Разработка микросервисной архитектуры на .NET 8. Работа с Kubernetes, RabbitMQ, PostgreSQL. Оптимизация производительности и рефакторинг легаси кода."
                    },
                    new WorkExperience
                    {
                        Company = "IT Solutions",
                        Position = "Middle .NET Developer",
                        StartDate = "2018-09",
                        EndDate = "2021-02",
                        Current = false,
                        Description = "Разработка и поддержка корпоративных приложений на ASP.NET Core. Интеграция с внешними API, оптимизация запросов к БД."
                    },
                    new WorkExperience
                    {
                        Company = "Веб-Студия",
                        Position = "Junior Developer",
                        StartDate = "2016-06",
                        EndDate = "2018-08",
                        Current = false,
                        Description = "Разработка сайтов на ASP.NET MVC, поддержка существующих проектов, работа с клиентами."
                    }
                },
                
                // Образование
                Education = new System.Collections.Generic.List<Education>
                {
                    new Education
                    {
                        Institution = "Московский Государственный Университет",
                        Degree = "Магистр",
                        Field = "Прикладная математика и информатика",
                        StartDate = "2014-09",
                        EndDate = "2016-06",
                        Current = false
                    },
                    new Education
                    {
                        Institution = "Московский Государственный Университет",
                        Degree = "Бакалавр",
                        Field = "Прикладная математика",
                        StartDate = "2010-09",
                        EndDate = "2014-06",
                        Current = false
                    }
                },
                
                // Навыки
                Skills = new System.Collections.Generic.List<string>
                {
                    "C#", ".NET Core", "ASP.NET", "Entity Framework", "Dapper",
                    "SQL", "PostgreSQL", "MongoDB", "Redis",
                    "Docker", "Kubernetes", "RabbitMQ",
                    "REST API", "gRPC", "GraphQL",
                    "Git", "GitHub Actions", "Jenkins",
                    "Azure", "AWS"
                }
            };

            try
            {
                // Создаём сервис
                var pdfService = new PdfGenerationService();
                
                // Путь для сохранения PDF
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var outputPath = Path.Combine(desktopPath, $"resume_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                
                Console.WriteLine($"\n📄 Генерация PDF...");
                Console.WriteLine($"📁 Будет сохранён на рабочий стол: {outputPath}");
                
                // Генерируем PDF
                var pdfBytes = await pdfService.GenerateResumePdfAsync(testData);
                
                // Сохраняем файл
                await File.WriteAllBytesAsync(outputPath, pdfBytes);
                
                Console.WriteLine($"✅ PDF успешно создан!");
                Console.WriteLine($"📊 Размер файла: {pdfBytes.Length} байт");
                Console.WriteLine($"📍 Открыть файл: {outputPath}");
                
                // Показываем краткую информацию о том, что сгенерировано
                Console.WriteLine("\n📋 Сгенерированные данные:");
                Console.WriteLine($"   Имя: {testData.FirstName} {testData.LastName}");
                Console.WriteLine($"   Email: {testData.Email}");
                Console.WriteLine($"   Опыт работы: {testData.WorkExperience.Count} мест");
                Console.WriteLine($"   Образование: {testData.Education.Count} записей");
                Console.WriteLine($"   Навыки: {testData.Skills.Count} шт.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ошибка: {ex.Message}");
                Console.WriteLine($"📌 Stack trace: {ex.StackTrace}");
            }
        }
    }
}