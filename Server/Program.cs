using System.Text.Json;

var builder = WebApplication
    .CreateBuilder(args);

var app = builder.Build();
app.MapGet("/stocks/{stock:alpha}", async (
        string stock, 
        HttpContext httpContext, 
        CancellationToken cancellationToken) =>
    {
        httpContext.Response.Headers.Append("Content-Type", "text/event-stream");
    
        while (!cancellationToken.IsCancellationRequested)
        {
            var stockItem = new
            {
                Stock = stock,
                Price = Random.Shared.Next(0, 1000),
                DateTime = DateTime.Now
            };
            
            await httpContext.Response.WriteAsync("event: stockChanged", cancellationToken: cancellationToken);
            await httpContext.Response.WriteAsync("\n", cancellationToken: cancellationToken);
            await httpContext.Response.WriteAsync("data: ", cancellationToken: cancellationToken);
            await JsonSerializer.SerializeAsync(httpContext.Response.Body, stockItem, cancellationToken: cancellationToken);
            await httpContext.Response.WriteAsync("\n\n", cancellationToken: cancellationToken);
            await httpContext.Response.Body.FlushAsync(cancellationToken);

            await Task.Delay(Random.Shared.Next(1000, 5000), cancellationToken);
        }
    })
    .WithName("GetWeatherForecast");

app.Run();