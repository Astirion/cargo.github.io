using System;
using System.ComponentModel.DataAnnotations;

namespace CargoGo.Dal.Entities;

public class SenderEntity
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

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string UserId { get; set; } = string.Empty; 
}
