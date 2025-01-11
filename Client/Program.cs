using System.Net.ServerSentEvents;

const string url = "http://localhost:5169/stocks/";

Console.WriteLine("Informe o símbolo da ação:");
var stock = Console.ReadLine();

using var client = new HttpClient();
await using var stream = await client.GetStreamAsync(url + stock);
await foreach (var item in SseParser.Create(stream).EnumerateAsync())
{
    if (item.EventType == "stockChanged")
        Console.WriteLine(item.Data);
}

Console.WriteLine("Fim...");