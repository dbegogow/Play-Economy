using Play.Catalog.Service.Entities;
using Play.Common.MongoDB;
using Play.Common.MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMongo()
    .AddMongoRepository<Item>("items")
    .AddMassTransiteWithRabbitMq()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers(options
        => options.SuppressAsyncSuffixInActionNames = false);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app
       .UseSwagger()
       .UseSwaggerUI();
}

app
   .UseHttpsRedirection()
   .UseAuthorization();

app.MapControllers();

app.Run();
