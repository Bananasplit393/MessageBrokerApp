using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Consumer.Interfaces;
using Consumer.Processing;


namespace Consumer;
public class MessageHandler : IMessageHandler, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IMessageProcessor _messageProcessor;
    private const string QueueName = "message_queue";

    public MessageHandler(IMessageProcessor messageProcessor)
    {
        _messageProcessor = messageProcessor;
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
    }

    public async Task HandleMessage(Shared.Models.Message message, CancellationToken cancellationToken)
    {
        try
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = System.Text.Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<Shared.Models.Message>(json);

                    if (message == null)
                    {
                        Console.WriteLine("Received null message");
                        return;
                    }

                    Console.WriteLine($"Received message: Content={message.Content}, Timestamp={message.Timestamp}, Counter={message.Counter}");

                    var result = await _messageProcessor.ProcessMessage(message);

                    Console.WriteLine($"Processing result: Action={result.Action}");

                    if (result.Action == ProcessingAction.Requeue) 
                    {
                        var updatedJson = JsonSerializer.Serialize(result.Message);
                        var updatedBody = System.Text.Encoding.UTF8.GetBytes(updatedJson);
                        _channel.BasicPublish("", QueueName, null, updatedBody);
                    }

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            _channel.BasicConsume(QueueName, false, consumer);

            await Task.Delay(-1, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in HandleMessage: {ex.Message}");
        }
    }
        public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
