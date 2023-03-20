using Play.Inventory.Service.Entities;
using Play.Common;
using Play.Catalog.Contracts;
using MassTransit;

namespace Play.Inventory.Service.Consumers;

public class CatalogItemDeletedConsumers : IConsumer<CatalogItemDeleted>
{
    private readonly IRepository<CatalogItem> repository;

    public CatalogItemDeletedConsumers(IRepository<CatalogItem> repository)
        => this.repository = repository;

    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        var message = context.Message;

        var item = await this.repository
            .GetAsync(message.ItemId);

        if (item == null)
        {
            return;
        }

        await this.repository
            .RemoveAsync(message.ItemId);
    }
}
