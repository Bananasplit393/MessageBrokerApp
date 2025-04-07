using Consumer;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var databaseService = new PostgresDatabaseService("Host=localhost;Database=messages;Username=postgres;Password=postgres");
using var processor = new MessageProcessor(databaseService);
await processor.ProcessMessages(cts.Token);
