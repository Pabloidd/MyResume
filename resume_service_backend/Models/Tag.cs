namespace resume_service_backend.Models
{
    /// <summary>
    /// Тег (навык) с категорией
    /// </summary>
 public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}