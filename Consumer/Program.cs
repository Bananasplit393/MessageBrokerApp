using Consumer;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var connectionString = "Host=localhost;Database=messages;Username=postgres;Password=postgres";
var databaseService = new PostgresDatabaseService(connectionString);

//Console.WriteLine("Ensuring database table exists...");

//await databaseService.EnsureTableExists();

Console.WriteLine("Starting message processing...");

using var processor = new MessageProcessor(databaseService);

Console.WriteLine("Processing messages...");

await processor.ProcessMessages(cts.Token);

Console.WriteLine("Message processing stopped.");