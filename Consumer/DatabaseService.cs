using Shared.Models;

namespace Consumer;

public interface IDatabaseService
{
    Task StoreMessage(Message message);
}

public class PostgresDatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public PostgresDatabaseService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task StoreMessage(Message message)
    {
        // Implement actual database storage logic here
        // This is just a placeholder
        Console.WriteLine($"Stored message in database: Timestamp={message.Timestamp}, Counter={message.Counter}");
    }
}
