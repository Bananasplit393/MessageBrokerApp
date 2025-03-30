using Producer;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

using var producer = new ProducerService();
await producer.StartProducing(cts.Token);
