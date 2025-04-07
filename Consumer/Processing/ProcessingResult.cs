using Shared.Models;

namespace Consumer.Processing;

public class ProcessingResult
{
    public ProcessingAction Action { get; set; }
    public Message? Message { get; set; }
}
