using RabbitMQ.Client;
using System.Text.Json;
using Shared.Models;

namespace Producer;

public class ProducerService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "message_queue";

    public ProducerService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
    }

    public async Task StartProducing(CancellationToken cancellationToken)
    {
        var counter = 0;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var message = new Message
            {
                Content = $"message {counter}",
                Timestamp = DateTime.UtcNow,
                Counter = counter++
            };

            var json = JsonSerializer.Serialize(message);
            var body = System.Text.Encoding.UTF8.GetBytes(json);
            
            _channel.BasicPublish("", QueueName, null, body);
            Console.WriteLine($"Sent message: Content={message.Content}, Timestamp={message.Timestamp}, Counter={message.Counter}");
            
            await Task.Delay(1000, cancellationToken); // 1 second delay
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
