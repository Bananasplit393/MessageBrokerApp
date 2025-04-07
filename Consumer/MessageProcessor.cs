using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Shared.Models;

namespace Consumer;

public class MessageProcessor : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IDatabaseService _databaseService;
    private const string QueueName = "message_queue";

    public MessageProcessor(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
    }

    public async Task ProcessMessages(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (model, ea) =>
        {
        var body = ea.Body.ToArray();
        var json = System.Text.Encoding.UTF8.GetString(body);
        var message = JsonSerializer.Deserialize<Message>(json);

        if (message == null)
        {
            Console.WriteLine("Received null message");
            return;
        }

        Console.WriteLine($"Received message: Content={message.Content}, Timestamp={message.Timestamp}, Counter={message.Counter}");

        var result = await ProcessMessage(message);

        Console.WriteLine($"Processing result: Action={result.Action}");

        if (result.Action == ProcessingAction.Requeue) 
        {
            var updatedJson = JsonSerializer.Serialize(result.Message);
            var updatedBody = System.Text.Encoding.UTF8.GetBytes(updatedJson);
            _channel.BasicPublish("", QueueName, null, updatedBody);
        }

        _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(QueueName, false, consumer);

        // Keep the consumer running
        await Task.Delay(-1, cancellationToken);
    }

    public async Task<ProcessingResult> ProcessMessage(Message message)
    {
        var messageAge = DateTime.UtcNow - message.Timestamp;
        
        // Discard messages older than 1 minute
        if (messageAge.TotalMinutes > 1)
        {
            return new ProcessingResult { Action = ProcessingAction.Discard };
        }

        if (message.ReQueueCounter >= 3)
        {
            Console.WriteLine($"Message discarded becuase RequeueCounter is greater than 3");
            return new ProcessingResult { Action = ProcessingAction.Discard };
        }

        // Check if seconds are even or odd
        if (message.Timestamp.Second % 2 == 0)
        {
            // Even seconds - store in database
            await _databaseService.StoreMessage(message);
            return new ProcessingResult { Action = ProcessingAction.Store };
        }
        else
        {
            // Odd seconds - increment counter and requeue
            message.Counter++;
            message.ReQueueCounter++;
            return new ProcessingResult 
            { 
                Action = ProcessingAction.Requeue,
                Message = message,
            };
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

public enum ProcessingAction
{
    Store,
    Requeue,
    Discard
}

public class ProcessingResult
{
    public ProcessingAction Action { get; set; }
    public Message? Message { get; set; }
}
