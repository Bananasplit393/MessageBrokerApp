using System;
using System.Threading;
using System.Threading.Tasks;
using Consumer;
using Shared.Models;

class Program
{
    static async Task Main(string[] args)
    {
        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };


        string connectionString = "Host=localhost;Database=messages;Username=postgres;Password=postgres";

        var messageProcessor = new MessageProcessor(new PostgresDatabaseService(connectionString));
        var messageHandler = new MessageHandler(messageProcessor);

        Console.WriteLine("Starting message processing...");

        try
        {
            await messageHandler.HandleMessage(new Message(), cts.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            messageHandler.Dispose();
            Console.WriteLine("Message processing stopped.");
        }
    }
}