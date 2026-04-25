using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using resume_service_backend.Models;

namespace resume_service_backend.Services
{
    /// <summary>
    /// PDF резюме (QuestPDF): двухколоночный западный шаблон, типографика и сетка под печать A4.
    /// </summary>
    public class PdfGenerationService : IPdfGenerationService
    {
        /// <summary>Windows/macOS с системным шрифтом; при отсутствии QuestPDF подставит метрику ближайшего sans.</summary>
        private const string BodyFont = "Segoe UI";

        private static readonly Color Paper = Color.FromRGB(255, 255, 255);
        private static readonly Color Ink = Color.FromRGB(20, 24, 33);
        private static readonly Color InkSoft = Color.FromRGB(55, 62, 78);
        private static readonly Color Muted = Color.FromRGB(107, 114, 128);
        private static readonly Color Accent = Color.FromRGB(30, 90, 125);
        private static readonly Color AccentSoft = Color.FromRGB(232, 241, 248);
        private static readonly Color Hairline = Color.FromRGB(229, 231, 235);
        private static readonly Color SidebarFill = Color.FromRGB(249, 250, 251);

        private static readonly TextStyle BodyStyle = TextStyle.Default
            .FontFamily(BodyFont)
            .FontSize(9.75f)
            .LineHeight(1.48f)
            .FontColor(InkSoft);

        static PdfGenerationService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateResumePdfAsync(ResumeData data)
        {
            return await Task.Run(() =>
            {
                var document = CreateDocument(data);
                return document.GeneratePdf();
            });
        }

        public async Task<string> GenerateAndSaveAsync(ResumeData data, string outputPath)
        {
            return await Task.Run(() =>
            {
                var document = CreateDocument(data);
                document.GeneratePdf(outputPath);
                return outputPath;
            });
        }

        private static bool TryParseYearMonth(string? raw, out int year, out int month)
        {
            year = 0;
            month = 0;
            if (string.IsNullOrWhiteSpace(raw)) return false;
            raw = raw.Trim();

            if (DateTime.TryParse(raw, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                    out var dtIso))
            {
                year = dtIso.Year;
                month = dtIso.Month;
                return month is >= 1 and <= 12;
            }

            var parts = raw.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 &&
                int.TryParse(parts[0], out year) &&
                int.TryParse(parts[1], out month))
                return month is >= 1 and <= 12;

            return false;
        }

        private static string FormatMonthYear(string? yyyyMmOrIso)
        {
            if (!TryParseYearMonth(yyyyMmOrIso, out var year, out var month))
                return (yyyyMmOrIso ?? "").Trim();

            var monthNames = new[]
            {
                "января", "февраля", "марта", "апреля", "мая", "июня",
                "июля", "августа", "сентября", "октября", "ноября", "декабря"
            };

            return $"{monthNames[month - 1]} {year}";
        }

        private static string FormatPeriod(string? start, string? end, bool current)
        {
            var a = FormatMonthYear(start);
            if (string.IsNullOrEmpty(a)) return "";
            var b = current ? "настоящее время" : FormatMonthYear(end);
            if (string.IsNullOrEmpty(b)) b = "—";
            return $"{a} — {b}";
        }

        private static IEnumerable<string> SplitDescriptionLines(string? description)
        {
            if (string.IsNullOrWhiteSpace(description)) yield break;
            var parts = description.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            var nonEmpty = parts.Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
            if (nonEmpty.Count == 0) yield break;
            if (nonEmpty.Count == 1 && nonEmpty[0].Length < 200)
            {
                yield return nonEmpty[0];
                yield break;
            }

            foreach (var line in nonEmpty)
                yield return line;
        }

        private static void ComposeMainColumn(ColumnDescriptor body, ResumeData data, List<string> skills, bool appendSkills)
        {
            body.Spacing(0);

            if (!string.IsNullOrWhiteSpace(data.About))
            {
                body.Item().PaddingBottom(16).Column(block =>
                {
                    SectionTitle(block, "О СЕБЕ");
                    block.Item().PaddingTop(7)
                        .Text(data.About.Trim())
                        .FontFamily(BodyFont)
                        .FontSize(9.75f)
                        .LineHeight(1.52f)
                        .FontColor(InkSoft)
                        .AlignLeft();
                });
            }

            if (data.WorkExperience is { Count: > 0 })
            {
                var experiences = data.WorkExperience
                    .Where(exp => !string.IsNullOrWhiteSpace(exp.Company) || !string.IsNullOrWhiteSpace(exp.Position))
                    .ToList();
                if (experiences.Count > 0)
                {
                    body.Item().PaddingBottom(4).Column(workBlock =>
                    {
                        SectionTitle(workBlock, "ОПЫТ РАБОТЫ");

                        foreach (var exp in experiences)
                        {
                            workBlock.Item().PaddingTop(12).Column(entry =>
                            {
                                var period = FormatPeriod(exp.StartDate, exp.EndDate, exp.Current);

                                entry.Item().Row(titleRow =>
                                {
                                    titleRow.RelativeItem().Text(t =>
                                    {
                                        if (!string.IsNullOrWhiteSpace(exp.Position))
                                            t.Span(exp.Position.Trim()).Bold().FontSize(11.25f).FontColor(Ink);
                                        else if (!string.IsNullOrWhiteSpace(exp.Company))
                                            t.Span(exp.Company.Trim()).Bold().FontSize(11.25f).FontColor(Ink);
                                    });

                                    if (!string.IsNullOrWhiteSpace(period))
                                    {
                                        titleRow.AutoItem().AlignRight().PaddingLeft(8)
                                            .Text(period)
                                            .FontFamily(BodyFont)
                                            .FontSize(8.25f)
                                            .SemiBold()
                                            .FontColor(Accent);
                                    }
                                });

                                if (!string.IsNullOrWhiteSpace(exp.Company) && !string.IsNullOrWhiteSpace(exp.Position))
                                {
                                    entry.Item().PaddingTop(3).Text(exp.Company.Trim())
                                        .FontFamily(BodyFont)
                                        .FontSize(9.35f)
                                        .SemiBold()
                                        .FontColor(Muted);
                                }
                                else if (!string.IsNullOrWhiteSpace(exp.Company))
                                {
                                    entry.Item().PaddingTop(3).Text(exp.Company.Trim())
                                        .FontFamily(BodyFont)
                                        .FontSize(9.35f)
                                        .SemiBold()
                                        .FontColor(Muted);
                                }

                                foreach (var line in SplitDescriptionLines(exp.Description))
                                {
                                    entry.Item().PaddingTop(5).Row(bullet =>
                                    {
                                        bullet.ConstantItem(12).PaddingTop(1).Text("▸")
                                            .FontFamily(BodyFont)
                                            .FontSize(8.5f)
                                            .FontColor(Accent);
                                        bullet.RelativeItem().Text(line)
                                            .FontFamily(BodyFont)
                                            .FontSize(9.15f)
                                            .LineHeight(1.45f)
                                            .FontColor(InkSoft);
                                    });
                                }
                            });
                        }
                    });
                }
            }

            if (data.Education is { Count: > 0 })
            {
                body.Item().PaddingTop(18).Column(eduBlock =>
                {
                    SectionTitle(eduBlock, "ОБРАЗОВАНИЕ");

                    foreach (var ed in data.Education)
                    {
                        if (string.IsNullOrWhiteSpace(ed.Institution)) continue;

                        eduBlock.Item().PaddingTop(12).Column(entry =>
                        {
                            var period = FormatPeriod(ed.StartDate, ed.EndDate, ed.Current);
                            if (!string.IsNullOrWhiteSpace(period))
                            {
                                entry.Item().Text(period)
                                    .FontFamily(BodyFont)
                                    .FontSize(8.25f)
                                    .SemiBold()
                                    .FontColor(Accent);
                            }

                            entry.Item().PaddingTop(3).Text(ed.Institution.Trim())
                                .FontFamily(BodyFont)
                                .FontSize(11.1f)
                                .Bold()
                                .FontColor(Ink);

                            var line = string.Join(" · ",
                                new[] { ed.Degree, ed.Field }.Where(s => !string.IsNullOrWhiteSpace(s)));
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                entry.Item().PaddingTop(3).Text(line)
                                    .FontFamily(BodyFont)
                                    .FontSize(9.35f)
                                    .FontColor(Muted);
                            }
                        });
                    }
                });
            }

            if (appendSkills && skills.Count > 0)
            {
                body.Item().PaddingTop(18).Column(skBlock =>
                {
                    SectionTitle(skBlock, "НАВЫКИ");
                    skBlock.Item().PaddingTop(6).Column(skillCol =>
                    {
                        skillCol.Spacing(5);
                        foreach (var s in skills)
                        {
                            skillCol.Item().Row(r =>
                            {
                                r.ConstantItem(10).Text("•")
                                    .FontFamily(BodyFont)
                                    .FontSize(9f)
                                    .FontColor(Accent);
                                r.RelativeItem().Text(s)
                                    .FontFamily(BodyFont)
                                    .FontSize(9.1f)
                                    .FontColor(InkSoft)
                                    .LineHeight(1.35f);
                            });
                        }
                    });
                });
            }
        }

        private Document CreateDocument(ResumeData data)
        {
            var fullName = $"{data.FirstName} {data.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = "Резюме";

            var skills = data.Skills?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList() ?? new List<string>();
            var hasContacts = !string.IsNullOrWhiteSpace(data.Email) || !string.IsNullOrWhiteSpace(data.Phone);
            var useSidebar = hasContacts || skills.Count > 0;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginLeft(1.15f, Unit.Centimetre);
                    page.MarginRight(1.15f, Unit.Centimetre);
                    page.MarginTop(1.05f, Unit.Centimetre);
                    page.MarginBottom(1f, Unit.Centimetre);
                    page.PageColor(Paper);

                    page.DefaultTextStyle(BodyStyle);

                    page.Content().Column(main =>
                    {
                        main.Spacing(0);

                        // ——— Шапка ———
                        main.Item().Column(header =>
                        {
                            header.Item().AlignCenter()
                                .Text(fullName)
                                .FontFamily(BodyFont)
                                .FontSize(26)
                                .Bold()
                                .FontColor(Ink);

                            if (!string.IsNullOrWhiteSpace(data.DesiredPosition))
                            {
                                header.Item().PaddingTop(5).AlignCenter()
                                    .Text(data.DesiredPosition.Trim())
                                    .FontFamily(BodyFont)
                                    .FontSize(11.25f)
                                    .SemiBold()
                                    .FontColor(Accent);
                            }

                            header.Item().PaddingTop(12)
                                .LineHorizontal(0.9f)
                                .LineColor(Accent);
                        });

                        // ——— Две колонки (или одна, если нет контактов и навыков) ———
                        main.Item().PaddingTop(14).Row(row =>
                        {
                            row.Spacing(0);

                            if (useSidebar)
                            {
                                row.RelativeItem(3).Background(SidebarFill)
                                    .BorderRight(0.65f)
                                    .BorderColor(Hairline)
                                    .PaddingVertical(14)
                                    .PaddingHorizontal(13).Column(side =>
                                    {
                                        side.Spacing(12);

                                        if (hasContacts)
                                        {
                                            SectionTitle(side, "КОНТАКТЫ");
                                            side.Item().Column(contacts =>
                                            {
                                                contacts.Spacing(9);
                                                ContactBlock(contacts, "Email", data.Email);
                                                ContactBlock(contacts, "Телефон", data.Phone);
                                            });
                                        }

                                        if (skills.Count > 0)
                                        {
                                            if (hasContacts) side.Item().PaddingTop(6);
                                            SectionTitle(side, "НАВЫКИ");
                                            side.Item().PaddingTop(6).Column(skillCol =>
                                            {
                                                skillCol.Spacing(5);
                                                foreach (var s in skills)
                                                {
                                                    skillCol.Item().Row(r =>
                                                    {
                                                        r.ConstantItem(10).Text("•")
                                                            .FontFamily(BodyFont)
                                                            .FontSize(9f)
                                                            .FontColor(Accent);
                                                        r.RelativeItem().Text(s)
                                                            .FontFamily(BodyFont)
                                                            .FontSize(9.1f)
                                                            .FontColor(InkSoft)
                                                            .LineHeight(1.35f);
                                                    });
                                                }
                                            });
                                        }
                                    });

                                row.RelativeItem(7).PaddingLeft(16).PaddingTop(2)
                                    .Column(body => ComposeMainColumn(body, data, skills, appendSkills: false));
                            }
                            else
                            {
                                row.RelativeItem(10).PaddingTop(2)
                                    .Column(body => ComposeMainColumn(body, data, skills, appendSkills: true));
                            }
                        });
                    });

                    page.Footer().Column(foot =>
                    {
                        foot.Item().LineHorizontal(0.35f).LineColor(Hairline);
                        foot.Item().PaddingTop(6).AlignCenter().Text(t =>
                        {
                            var when = data.CreatedAt != default
                                ? data.CreatedAt.ToString("dd.MM.yyyy HH:mm")
                                : DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                            t.Span($"Сформировано: {when}")
                                .FontFamily(BodyFont)
                                .FontSize(7.25f)
                                .FontColor(Muted);
                        });
                    });
                });
            });
        }

        private static void SectionTitle(ColumnDescriptor column, string titleUpper)
        {
            column.Item().Row(r =>
            {
                r.ConstantItem(3.2f).Height(11).Background(Accent);
                r.RelativeItem().PaddingLeft(8).AlignMiddle()
                    .Text(titleUpper)
                    .FontFamily(BodyFont)
                    .FontSize(7.25f)
                    .Bold()
                    .FontColor(Muted);
            });
        }

        private static void ContactBlock(ColumnDescriptor col, string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            col.Item().Background(AccentSoft).PaddingVertical(7).PaddingHorizontal(9).Column(c =>
            {
                c.Item().Text(label)
                    .FontFamily(BodyFont)
                    .FontSize(6.75f)
                    .Bold()
                    .FontColor(Muted);
                c.Item().PaddingTop(3).Text(value.Trim())
                    .FontFamily(BodyFont)
                    .FontSize(9.35f)
                    .SemiBold()
                    .FontColor(Ink)
                    .LineHeight(1.25f);
            });
        }
    }
}
