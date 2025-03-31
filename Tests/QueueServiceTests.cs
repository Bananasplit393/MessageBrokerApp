using RabbitMQ.Client;
using Xunit;

public class QueueServiceTests
{
    [Fact]
    public void CanConnectToRabbitMQ()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        Assert.NotNull(connection);
    }
}
