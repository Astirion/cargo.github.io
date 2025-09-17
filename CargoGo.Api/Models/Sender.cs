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
    public string From { get; set; } = string.Empty;
    
    /// <summary>
    /// Город назначения
    /// </summary>
    public string To { get; set; } = string.Empty;
    
    /// <summary>
    /// Вес груза в килограммах
    /// </summary>
    public double Weight { get; set; }
    
    /// <summary>
    /// Описание груза
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
