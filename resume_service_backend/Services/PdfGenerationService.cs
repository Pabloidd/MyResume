using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using resume_service_backend.Models;

namespace resume_service_backend.Services
{
    /// <summary>
    /// Сервис генерации PDF с использованием QuestPDF
    /// </summary>
    public class PdfGenerationService : IPdfGenerationService
    {
        // Настройка лицензии QuestPDF (бесплатная для некоммерческого использования)
        static PdfGenerationService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Генерирует PDF-резюме в виде байтового массива
        /// </summary>
        public async Task<byte[]> GenerateResumePdfAsync(ResumeData data)
        {
            return await Task.Run(() =>
            {
                var document = CreateDocument(data);
                return document.GeneratePdf();
            });
        }

        /// <summary>
        /// Генерирует PDF и сохраняет в файл (для отладки)
        /// </summary>
        public async Task<string> GenerateAndSaveAsync(ResumeData data, string outputPath)
        {
            return await Task.Run(() =>
            {
                var document = CreateDocument(data);
                document.GeneratePdf(outputPath);
                return outputPath;
            });
        }

        /// <summary>
        /// Преобразует дату из формата YYYY-MM в читаемый вид
        /// </summary>
        private string FormatDate(string? yyyyMm)
        {
            if (string.IsNullOrWhiteSpace(yyyyMm)) return "";
            
            var parts = yyyyMm.Split('-');
            if (parts.Length != 2) return yyyyMm;
            
            if (!int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month))
                return yyyyMm;
            
            if (month < 1 || month > 12) return yyyyMm;
            
            var monthNames = new[] { 
                "января", "февраля", "марта", "апреля", "мая", "июня", 
                "июля", "августа", "сентября", "октября", "ноября", "декабря" 
            };
            
            return $"{monthNames[month - 1]} {year}";
        }

        /// <summary>
        /// Создаёт документ QuestPDF на основе данных резюме
        /// </summary>
        private Document CreateDocument(ResumeData data)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Настройки страницы A4
                    page.Size(PageSizes.A4);
                    page.Margin(2f, Unit.Centimetre);
                    
                    // Основной шрифт
                    page.DefaultTextStyle(x => x
                        .FontFamily("Times New Roman")
                        .FontSize(11f)
                        .LineHeight(1.5f));

                    // ===== ШАПКА (только на первой странице) =====
                    page.Header().ShowOnce().Column(header =>
                    {
                        // Имя и фамилия
                        header.Item().Text($"{data.FirstName} {data.LastName}")
                            .FontSize(36)
                            .Bold()
                            .FontColor(Colors.Black);
                        
                        // Желаемая должность - нормальный интервал
                        if (!string.IsNullOrWhiteSpace(data.DesiredPosition))
                        {
                            header.Item().PaddingTop(5).Text(data.DesiredPosition)
                                .FontSize(14)
                                .FontColor(Colors.Grey.Medium)
                                .SemiBold();
                        }

                        // Контакты без иконок (просто текст)
                        header.Item().PaddingTop(15).Row(contacts =>
                        {
                            if (!string.IsNullOrWhiteSpace(data.Email))
                            {
                                contacts.AutoItem().Text(data.Email);
                            }
                            
                            if (!string.IsNullOrWhiteSpace(data.Phone))
                            {
                                contacts.AutoItem().PaddingLeft(20).Text(data.Phone);
                            }
                        });

                        // Декоративная линия
                        header.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    // ===== СОДЕРЖИМОЕ =====
                    page.Content().Column(content =>
                    {
                        // ----- О СЕБЕ -----
                        if (!string.IsNullOrWhiteSpace(data.About))
                        {
                            content.Item().Column(about =>
                            {
                                about.Item().Text("О СЕБЕ")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Black);
                                
                                about.Item().PaddingTop(8).PaddingLeft(10).Text(data.About)
                                    .FontSize(11);
                                
                                about.Item().PaddingTop(15);
                            });
                        }

                        // ----- ОПЫТ РАБОТЫ -----
                        if (data.WorkExperience != null && data.WorkExperience.Count > 0)
                        {
                            content.Item().Column(work =>
                            {
                                work.Item().Text("ОПЫТ РАБОТЫ")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Black);

                                foreach (var exp in data.WorkExperience)
                                {
                                    if (string.IsNullOrWhiteSpace(exp.Company) && 
                                        string.IsNullOrWhiteSpace(exp.Position))
                                        continue;

                                    work.Item().PaddingTop(12).Row(row =>
                                    {
                                        // Левая колонка: даты
                                        row.AutoItem().Width(120).PaddingRight(15).Column(dates =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(exp.StartDate))
                                            {
                                                var start = FormatDate(exp.StartDate);
                                                var end = exp.Current ? "настоящее время" : FormatDate(exp.EndDate);
                                                
                                                dates.Item().Text($"{start} — {end}")
                                                    .FontSize(10)
                                                    .Bold()
                                                    .FontColor(Colors.Grey.Darken1);
                                            }
                                        });

                                        // Правая колонка: компания и должность
                                        row.RelativeItem().Column(details =>
                                        {
                                            details.Item().Text(exp.Position)
                                                .FontSize(12)
                                                .Bold();
                                            
                                            details.Item().Text(exp.Company)
                                                .FontSize(11)
                                                .FontColor(Colors.Grey.Darken1);

                                            if (!string.IsNullOrWhiteSpace(exp.Description))
                                            {
                                                details.Item().PaddingTop(5).Text(exp.Description)
                                                    .FontSize(10);
                                            }
                                        });
                                    });
                                }
                                work.Item().PaddingTop(10);
                            });
                        }

                        // ----- ОБРАЗОВАНИЕ -----
                        if (data.Education != null && data.Education.Count > 0)
                        {
                            content.Item().PaddingTop(15).Column(edu =>
                            {
                                edu.Item().Text("ОБРАЗОВАНИЕ")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Black);

                                foreach (var ed in data.Education)
                                {
                                    if (string.IsNullOrWhiteSpace(ed.Institution))
                                        continue;

                                    edu.Item().PaddingTop(10).Row(row =>
                                    {
                                        // Левая колонка: даты
                                        row.AutoItem().Width(120).PaddingRight(15).Column(dates =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(ed.StartDate))
                                            {
                                                var start = FormatDate(ed.StartDate);
                                                var end = ed.Current ? "настоящее время" : FormatDate(ed.EndDate);
                                                
                                                dates.Item().Text($"{start} — {end}")
                                                    .FontSize(10)
                                                    .Bold()
                                                    .FontColor(Colors.Grey.Darken1);
                                            }
                                        });

                                        // Правая колонка: учебное заведение
                                        row.RelativeItem().Column(details =>
                                        {
                                            details.Item().Text(ed.Institution)
                                                .FontSize(12)
                                                .Bold();
                                            
                                            var degreeText = "";
                                            if (!string.IsNullOrWhiteSpace(ed.Degree))
                                                degreeText += ed.Degree;
                                            if (!string.IsNullOrWhiteSpace(ed.Field))
                                                degreeText += string.IsNullOrWhiteSpace(degreeText) ? ed.Field : $", {ed.Field}";
                                            
                                            if (!string.IsNullOrWhiteSpace(degreeText))
                                            {
                                                details.Item().PaddingTop(3).Text(degreeText)
                                                    .FontSize(11)
                                                    .FontColor(Colors.Grey.Darken1);
                                            }
                                        });
                                    });
                                }
                                edu.Item().PaddingTop(5);
                            });
                        }

                        // ----- НАВЫКИ -----
                        if (data.Skills != null && data.Skills.Count > 0)
                        {
                            content.Item().PaddingTop(15).Column(skills =>
                            {
                                skills.Item().Text("НАВЫКИ")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(Colors.Black);

                                // Навыки в две колонки (без иконок, просто точки)
                                var midPoint = (int)Math.Ceiling(data.Skills.Count / 2.0);
                                var leftSkills = data.Skills.Take(midPoint).ToList();
                                var rightSkills = data.Skills.Skip(midPoint).ToList();

                                skills.Item().PaddingTop(8).Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        foreach (var skill in leftSkills)
                                        {
                                            col.Item().PaddingBottom(4).Text($"• {skill}")
                                                .FontSize(11);
                                        }
                                    });

                                    row.RelativeItem().Column(col =>
                                    {
                                        foreach (var skill in rightSkills)
                                        {
                                            col.Item().PaddingBottom(4).Text($"• {skill}")
                                                .FontSize(11);
                                        }
                                    });
                                });
                            });
                        }
                    });

                    // НИЖНИЙ КОЛОНТИТУЛ ПОЛНОСТЬЮ УДАЛЁН
                });
            });
        }
    }
}