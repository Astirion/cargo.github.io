using System;
using System.ComponentModel.DataAnnotations;

namespace CargoGo.Dal.Entities;

public class TravelerEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string From { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string To { get; set; } = string.Empty;

    [Range(0.1, 1000)]
    public double Weight { get; set; }

    [Range(1, 1000000)]
    public int Reward { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
