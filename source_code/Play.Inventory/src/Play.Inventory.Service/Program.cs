using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Clients;
using Play.Common.MongoDB;
using Polly;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddMongo();
builder.Services.AddMongoRepository<InventoryItem>("inventoryitems");

builder.Services
    .AddHttpClient<CatalogClient>(client
        => client.BaseAddress = new Uri("https://localhost:7281"))
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
