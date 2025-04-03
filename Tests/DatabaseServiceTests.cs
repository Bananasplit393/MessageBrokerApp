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
        var connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=messages"; // Needs changing
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        Assert.True(connection.State == System.Data.ConnectionState.Open);
    }
}
