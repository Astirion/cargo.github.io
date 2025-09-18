using System.ComponentModel.DataAnnotations;

namespace CargoGo.Api.Models;

/// <summary>
/// Модель отправителя груза
/// </summary>
public class Sender
{
    /// <summary>
    /// Уникальный идентификатор отправителя
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Город отправления
    /// </summary>
    /// <example>Казань</example>
    [Required(ErrorMessage = "Город отправления обязателен")]
    [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
    public string From { get; set; } = string.Empty;
    
    /// <summary>
    /// Город назначения
    /// </summary>
    /// <example>Екатеринбург</example>
    [Required(ErrorMessage = "Город назначения обязателен")]
    [StringLength(100, ErrorMessage = "Название города не должно превышать 100 символов")]
    public string To { get; set; } = string.Empty;
    
    /// <summary>
    /// Вес груза в килограммах
    /// </summary>
    /// <example>8.0</example>
    [Range(0.1, 1000, ErrorMessage = "Вес должен быть от 0.1 до 1000 кг")]
    public double Weight { get; set; }
    
    /// <summary>
    /// Описание груза
    /// </summary>
    /// <example>Документы и небольшие посылки</example>
    [Required(ErrorMessage = "Описание груза обязательно")]
    [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
