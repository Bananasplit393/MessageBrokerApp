using Shared.Models;
using Npgsql;

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

        if (message.Content == null )
            {
                throw new ArgumentNullException("Message content is null");
            }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var query = "INSERT INTO public.messages (content, created_at) VALUES (@content, @created_at)";
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@content", message.Content);
        command.Parameters.AddWithValue("@created_at", message.Timestamp);

        await command.ExecuteNonQueryAsync();
    }
}

