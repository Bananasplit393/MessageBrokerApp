using Shared.Models;
using Consumer.Processing;
using Consumer.Interfaces;

namespace Consumer;

public class MessageProcessor : IMessageProcessor, IDisposable
{
    private readonly IDatabaseService _databaseService;

    public MessageProcessor(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<ProcessingResult> ProcessMessage(Message message)
    {
        var messageAge = DateTime.UtcNow - message.Timestamp;
        
        if (messageAge.TotalMinutes > 1 || message.ReQueueCounter >= 3)
        {
            Console.WriteLine("Message discarded");
            return new ProcessingResult { Action = ProcessingAction.Discard };
        }

        if (message.Timestamp.Second % 2 == 0)
        {
            await _databaseService.StoreMessage(message);
            return new ProcessingResult { Action = ProcessingAction.Store };
        }
        else
        {
            message.Counter++;
            message.ReQueueCounter++;
            return new ProcessingResult { Action = ProcessingAction.Requeue, Message = message };
        }
    }

    // Implement IDisposable to 
    public void Dispose()
    {
        // Dispose of any resources if necessary
    }
}

