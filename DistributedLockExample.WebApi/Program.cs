using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RedisDistributedSynchronizationProvider>(_ =>
{
    var connectionString = builder.Configuration.GetConnectionString("RedisCache");
    var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString!);
    return new RedisDistributedSynchronizationProvider(connectionMultiplexer.GetDatabase());
});

var app = builder.Build();

app.MapGet("/locks", async (
    [FromServices] RedisDistributedSynchronizationProvider lockProvider,
    [FromQuery]string requestId
    ) => 
{
    Console.WriteLine($"Request {requestId} received.");

    const string lockKey = "SomeWellKnownKey";
    var lockTimeout = TimeSpan.FromSeconds(30);
    
    await using var @lock = await lockProvider.TryAcquireLockAsync(lockKey, lockTimeout);

    if (@lock == null)
    {
        return Results.Conflict(requestId);
    }

    await ExecuteCriticalLogicAsync(requestId);
    return Results.Ok(requestId);
});

app.Run();

return;

static async Task ExecuteCriticalLogicAsync(string requestId)
{
    Console.WriteLine($"Processing request {requestId} ...");
    var ms = Random.Shared.Next(5000, 10000);
    await Task.Delay(ms);
    Console.WriteLine($"Processing request {requestId} ... DONE. Elapsed ms {ms}.");
}
