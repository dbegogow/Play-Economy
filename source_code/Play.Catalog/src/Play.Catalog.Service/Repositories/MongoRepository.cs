using Play.Catalog.Service.Entities;
using MongoDB.Driver;

namespace Play.Catalog.Service.Repositories;

public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoCollection<T> dbCollection;
    private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

    public MongoRepository(
        IMongoDatabase database,
        string collectionName)
        => this.dbCollection = database.GetCollection<T>(collectionName);

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
        => await this.dbCollection
            .Find(this.filterBuilder.Empty)
            .ToListAsync();

    public async Task<T> GetAsync(Guid id)
    {
        var filter = this.filterBuilder
            .Eq(entity => entity.Id, id);

        return await this.dbCollection
            .Find(filter)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentException(nameof(entity));
        }

        await this.dbCollection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
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
