using Npgsql;
using Xunit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

public class DatabaseServiceTests
{
    [Fact]
    public void CanConnectToDatabase()
    {
        var connectionString = "Host=localhost;Database=messages;Username=postgres;Password=postgres";
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        Assert.True(connection.State == System.Data.ConnectionState.Open);
    }
}
