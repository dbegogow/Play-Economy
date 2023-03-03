using Play.Catalog.Service.Entities;
using MongoDB.Driver;
using SharpCompress.Common;

namespace Play.Catalog.Service.Repositories;

public class ItemsRepository
{
    private const string collectionName = "items";
    private readonly IMongoCollection<Item> dbCollection;
    private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

    public ItemsRepository()
    {
        var mongoClient = new MongoClient("mongodb://localhost:27017");
        var database = mongoClient.GetDatabase("Catalog");

        dbCollection = database.GetCollection<Item>(collectionName);
    }

    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        => await this.dbCollection
            .Find(filterBuilder.Empty)
            .ToListAsync();

    public async Task<Item> GetAsync(Guid id)
    {
        var filter = filterBuilder
            .Eq(entity => entity.Id, id);

        return await dbCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Item entity)
    {
        if (entity == null)
        {
            throw new ArgumentException(nameof(entity));
        }

        await dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Item entity)
    {
        if (entity == null)
        {
            throw new ArgumentException(nameof(entity));
        }

        var filter = filterBuilder
            .Eq(existingEntity => existingEntity.Id, entity.Id);

        await dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = filterBuilder
            .Eq(entity => entity.Id, id);

        await dbCollection.DeleteOneAsync(filter);
    }
}
