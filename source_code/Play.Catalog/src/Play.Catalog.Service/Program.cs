using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Settings;
using Play.Common.MongoDB;
using Play.Common.Settings;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddMongo();
builder.Services.AddMongoRepository<Item>("items");

builder.Services.AddMassTransit(c =>
{
    c.UsingRabbitMq((context, configurator) =>
    {
        var rabbitMQSettings = configuration
            .GetSection(nameof(RabbitMQSettings))
            .Get<RabbitMQSettings>();

        var serviceSettings = configuration
            .GetSection(nameof(ServiceSettings))
            .Get<ServiceSettings>();

        configurator.Host(rabbitMQSettings.Host);

        configurator.ConfigureEndpoints(
            context,
            new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
    });
});

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

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
