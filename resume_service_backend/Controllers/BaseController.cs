using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace resume_service_backend.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult? RequireString(string? value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return BadRequest($"{parameterName} не может быть пустым");
            return null;
        }

        protected ActionResult? RequireMin<T>(T value, T minValue, string parameterName) where T : IComparable<T>
        {
            if (value.CompareTo(minValue) < 0)
                return BadRequest($"{parameterName} должен быть не меньше {minValue}");
            return null;
        }

        protected ActionResult? RequirePositive<T>(T value, string parameterName) where T : IComparable<T>
        {
            if (value.CompareTo(default!) <= 0)
                return BadRequest($"{parameterName} должен быть положительным числом");
            return null;
        }

        /// <summary>
        /// УНИВЕРСАЛЬНЫЙ МЕТОД для выполнения любых операций с репозиторием.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемых данных (может быть object, List<T>, T или Unit)</typeparam>
        /// <param name="action">Функция, возвращающая Task<T></param>
        /// <param name="notFoundMessage">Сообщение для 404 (если null означает "не найдено")</param>
        /// <param name="successStatusCode">HTTP статус при успехе (200 по умолчанию)</param>
        /// <returns>ActionResult с соответствующим статусом и данными/сообщением</returns>
        protected async Task<ActionResult> ExecuteAsync<T>(
            Func<Task<T>> action,
            string? notFoundMessage = null,
            int successStatusCode = 200)
        {
            try
            {
                var result = await action();

                // Если результат null И мы ожидаем, что null = "не найдено"
                if (result == null && notFoundMessage != null)
                    return NotFound(notFoundMessage);

                // Для void-операций (где T = object или Unit)
                if (result is Unit)
                    return StatusCode(successStatusCode, "Операция выполнена успешно");

                // Для всех остальных случаев возвращаем данные
                return StatusCode(successStatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка сервера: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Вспомогательный тип для void-операций (аналог void в generic)
    /// </summary>
    public readonly struct Unit
    {
        public static readonly Unit Value = new Unit();
    }
}