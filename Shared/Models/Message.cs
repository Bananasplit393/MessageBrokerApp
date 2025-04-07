namespace Shared.Models;

public class Message
{
    public DateTime Timestamp { get; set; }
    public int Counter { get; set; }
    public string? Content { get; set; }
    public int ReQueueCounter { get; set; }
} 