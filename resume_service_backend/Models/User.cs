namespace resume_service_backend.Models
{
    /// <summary>
    /// Пользователь (только email, авторизация в отдельной БД)
    /// </summary>
    public class User
    {
        public string Email { get; set; } = string.Empty;
    }
}