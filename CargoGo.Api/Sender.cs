public class Sender
{
    public int Id { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public double Weight { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}