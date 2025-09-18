using System.ComponentModel.DataAnnotations;

namespace CargoGo.Api.Models;

/// <summary>
/// Модель путешественника
/// </summary>
public class Traveler
{
    /// <summary>
    /// Уникальный идентификатор путешественника
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Город отправления
    /// </summary>
    /// <example>Москва</example>
    [Required(ErrorMessage = "Город отправления обязателен")]
    [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
    public string From { get; set; } = string.Empty;
    
    /// <summary>
    /// Город назначения
    /// </summary>
    /// <example>Санкт-Петербург</example>
    [Required(ErrorMessage = "Город назначения обязателен")]
    [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
    public string To { get; set; } = string.Empty;
    
    /// <summary>
    /// Максимальный вес груза в килограммах
    /// </summary>
    /// <example>15.5</example>
    [Range(0.1, 1000, ErrorMessage = "Вес должен быть от 0.1 до 1000 кг")]
    public double Weight { get; set; }
    
    /// <summary>
    /// Вознаграждение в рублях
    /// </summary>
    /// <example>2500</example>
    [Range(1, 1000000, ErrorMessage = "Вознаграждение должно быть от 1 до 1,000,000 рублей")]
    public int Reward { get; set; }
    
    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
