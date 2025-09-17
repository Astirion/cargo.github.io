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
    public string From { get; set; } = string.Empty;
    
    /// <summary>
    /// Город назначения
    /// </summary>
    public string To { get; set; } = string.Empty;
    
    /// <summary>
    /// Максимальный вес груза в килограммах
    /// </summary>
    public double Weight { get; set; }
    
    /// <summary>
    /// Вознаграждение в рублях
    /// </summary>
    public int Reward { get; set; }
    
    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
