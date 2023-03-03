using Play.Catalog.Service.Entities;
using MongoDB.Driver;

namespace Play.Catalog.Service.Repositories;

public class ItemsRepository : IItemsRepository
{
    private const string collectionName = "items";
    private readonly IMongoCollection<Item> dbCollection;
    private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

    public ItemsRepository(IMongoDatabase database)
        => this.dbCollection = database.GetCollection<Item>(collectionName);

    public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        => await this.dbCollection
            .Find(this.filterBuilder.Empty)
            .ToListAsync();

    public async Task<Item> GetAsync(Guid id)
    {
        var filter = this.filterBuilder
            .Eq(entity => entity.Id, id);

        return await this.dbCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Item entity)
    {
        if (entity == null)
        {
            throw new ArgumentException(nameof(entity));
        }

        await this.dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Item entity)
    {
        if (entity == null)
        {
            throw new ArgumentException(nameof(entity));
        }

        var filter = this.filterBuilder
            .Eq(existingEntity => existingEntity.Id, entity.Id);

        await this.dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var filter = this.filterBuilder
            .Eq(entity => entity.Id, id);

        await this.dbCollection.DeleteOneAsync(filter);
    }
}
