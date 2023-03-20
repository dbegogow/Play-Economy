using Play.Inventory.Service.Entities;
using Play.Common;
using Play.Catalog.Contracts;
using MassTransit;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemCreatedConsumers : IConsumer<CatalogItemCreated>
{
    private readonly IRepository<CatalogItem> repository;

    public CatalogItemCreatedConsumers(IRepository<CatalogItem> repository)
        => this.repository = repository;

    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;

        var item = await this.repository
            .GetAsync(message.ItemId);

        if (item != null)
        {
            return;
        }

        item = new CatalogItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        };

        await repository.CreateAsync(item);
    }
}
