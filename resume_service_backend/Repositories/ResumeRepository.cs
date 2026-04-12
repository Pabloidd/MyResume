using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using MySqlConnector;
using resume_service_backend.Models;
using resume_service_backend.Options;

namespace resume_service_backend.Repositories
{
    public class ResumeRepository : AbstractRepository, IResumeRepository
    {
        public ResumeRepository(IOptions<MariaDbOptions> options) : base(options) { }

        /// <summary>
        /// Получить все резюме пользователя (sp_get_resumes_by_email)
        /// </summary>
        public async Task<List<ResumeBasic>> GetByEmailAsync(string email)
        {
            return await QueryProcAsync<ResumeBasic>(
                "sp_get_resumes_by_email",
                new { p_email = email }
            );
        }

        /// <summary>
        /// Получить публичные резюме по тегам (sp_get_public_resumes_by_tags)
        /// </summary>
        public async Task<List<PublicResume>> GetPublicByTagsAsync(string? tagIds = null)
        {
            return await QueryProcAsync<PublicResume>(
                "sp_get_public_resumes_by_tags",
                new { p_tag_ids = tagIds }
            );
        }

        /// <summary>
        /// Получить ВСЕ резюме (включая приватные) по тегам для АДМИНИСТРАТОРА
        /// </summary>
        public async Task<List<PublicResume>> GetAllByTagsForAdminAsync(string? tagIds = null)
        {
            using var connection = new MySqlConnection(_connectionString);
            
            // Если теги не переданы, получаем всё
            if (string.IsNullOrWhiteSpace(tagIds))
            {
                var query = @"
                    SELECT r.id as Id, r.pdf_filename as Name, r.email as Email,
                           COALESCE(GROUP_CONCAT(t.name SEPARATOR ', '), '') as Tags
                    FROM resumes r
                    LEFT JOIN resume_tags rt ON r.id = rt.resume_id
                    LEFT JOIN tags t ON rt.tag_id = t.id
                    GROUP BY r.id
                    ORDER BY r.id DESC";
                return (await connection.QueryAsync<PublicResume>(query)).ToList();
            }
            
            // Если переданы теги - фильтруем по ним
            var tagArray = tagIds.Split(',').Select(t => int.Parse(t.Trim())).ToList();
            var tagsQuery = @"
                SELECT r.id as Id, r.pdf_filename as Name, r.email as Email,
                       COALESCE(GROUP_CONCAT(t.name SEPARATOR ', '), '') as Tags
                FROM resumes r
                LEFT JOIN resume_tags rt ON r.id = rt.resume_id
                LEFT JOIN tags t ON rt.tag_id = t.id
                WHERE r.id IN (
                    SELECT resume_id FROM resume_tags WHERE tag_id IN @TagIds
                )
                GROUP BY r.id
                ORDER BY r.id DESC";
                
            return (await connection.QueryAsync<PublicResume>(tagsQuery, new { TagIds = tagArray })).ToList();
        }

        /// <summary>
        /// Получить ID и названия всех резюме (sp_get_all_resumes_basic)
        /// </summary>
        public async Task<List<ResumeBasic>> GetAllBasicAsync()
        {
            return await QueryProcAsync<ResumeBasic>("sp_get_all_resumes_basic");
        }

        /// <summary>
        /// Переключить статус резюме (sp_toggle_resume_status)
        /// </summary>
        public async Task<string> ToggleStatusAsync(int resumeId)
        {
            var result = await QuerySingleProcAsync<dynamic>(
                "sp_toggle_resume_status",
                new { p_resume_id = resumeId }
            );
            
            return result?.status ?? "unknown";
        }

        /// <summary>
        /// Создать новое резюме с тегами (sp_create_resume_with_tags)
        /// </summary>
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

        public async Task<Resume?> GetByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
    
            // Временный вариант — получаем как dynamic
            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                @"SELECT id, email, status, pdf_data, pdf_filename 
          FROM resumes WHERE id = @id",
                new { id }
            );
    
            if (result == null)
                return null;
    
            var resume = new Resume
            {
                Id = result.id,
                Email = result.email,
                Status = result.status,
                PdfData = result.pdf_data ?? Array.Empty<byte>(), // Явно приводим
                PdfFilename = result.pdf_filename
            };
    
            var tags = await connection.QueryAsync<Tag>(
                @"SELECT t.id, t.name, t.category 
          FROM tags t
          JOIN resume_tags rt ON t.id = rt.tag_id
          WHERE rt.resume_id = @id
          ORDER BY t.name",
                new { id }
            );
    
            resume.Tags = tags.ToList();
            return resume;
        }

        /// <summary>
        /// Удалить резюме по ID
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "DELETE FROM resumes WHERE id = @id",
                new { id }
            );
        }

        /// <summary>
        /// Обновить существующее резюме
        /// </summary>
        public async Task UpdateAsync(UpdateResumeRequest request)
        {
            // ИСПРАВЛЕНО: Вызываем protected метод из базового класса
            await ExecuteInTransactionAsync(async (connection, transaction) =>
            {
                // Обновляем основные поля резюме
                if (request.PdfData != null && !string.IsNullOrWhiteSpace(request.PdfFilename))
                {
                    await connection.ExecuteAsync(
                        @"UPDATE resumes 
                          SET status = @status, 
                              pdf_data = @pdf_data, 
                              pdf_filename = @pdf_filename 
                          WHERE id = @id",
                        new
                        {
                            request.Id,
                            request.Status,
                            request.PdfData,
                            request.PdfFilename
                        },
                        transaction
                    );
                }
                else
                {
                    await connection.ExecuteAsync(
                        @"UPDATE resumes 
                          SET status = @status 
                          WHERE id = @id",
                        new { request.Id, request.Status },
                        transaction
                    );
                }

                // Обновляем теги, если они переданы
                if (!string.IsNullOrWhiteSpace(request.TagIds))
                {
                    // Удаляем старые связи
                    await connection.ExecuteAsync(
                        "DELETE FROM resume_tags WHERE resume_id = @id",
                        new { request.Id },
                        transaction
                    );

                    // Добавляем новые теги
                    var tagIdsArray = request.TagIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var tagId in tagIdsArray)
                    {
                        if (int.TryParse(tagId, out var parsedTagId))
                        {
                            await connection.ExecuteAsync(
                                @"INSERT INTO resume_tags (resume_id, tag_id) 
                                  VALUES (@resumeId, @tagId)",
                                new { resumeId = request.Id, tagId = parsedTagId },
                                transaction
                            );
                        }
                    }
                }

                return true;
            });
        }

        /// <summary>
        /// Получить количество резюме пользователя
        /// </summary>
        public async Task<int> GetCountByEmailAsync(string email)
        {
            using var connection = new MySqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM resumes WHERE email = @email",
                new { email }
            );
        }
    }
}