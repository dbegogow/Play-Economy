using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Settings;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Play.Catalog.Service.Repositories;

public static class Extensions
{
    public static IServiceCollection AddMongo(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        var serviceSettings = configuration
            .GetSection(nameof(ServiceSettings))
            .Get<ServiceSettings>();

        services.AddSingleton(serviceProvider =>
        {
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

            return mongoClient.GetDatabase(serviceSettings.ServiceName);
        });

        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(
        this IServiceCollection services,
        string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database, collectionName);
        });

        return services;
    }
}
