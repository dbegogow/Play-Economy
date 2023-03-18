using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Clients;
using Play.Common.MongoDB;
using Polly;
using Polly.Timeout;
using System;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddMongo();
builder.Services.AddMongoRepository<InventoryItem>("inventoryitems");

var random = new Random();

builder.Services
    .AddHttpClient<CatalogClient>(client
        => client.BaseAddress = new Uri("https://localhost:7281"))
    .AddTransientHttpErrorPolicy(policyBuilder
        => policyBuilder
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(random.Next(0, 1000)),
                onRetry: (outcome, timespan, retryAttempt) =>
                    Console.WriteLine(
                        $"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}")
                ))
    .AddTransientHttpErrorPolicy(policyBuilder
        => policyBuilder
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(15),
                onBreak: (outcome, timespan) =>
                    Console.WriteLine(
                        $"Opening the circuit for {timespan.TotalSeconds} seconds..."),
                onReset: () =>
                    Console.WriteLine(
                        $"Closing the circuit...")
                ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
