using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> inventoryItemsRepository;
    private readonly IRepository<CatalogItem> catalogItemsRepository;

    public ItemsController(
        IRepository<InventoryItem> inventoryItemsRepository,
        IRepository<CatalogItem> catalogItemsRepository)
    {
        this.inventoryItemsRepository = inventoryItemsRepository;
        this.catalogItemsRepository = catalogItemsRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var inventoryItemEntities = await inventoryItemsRepository
            .GetAllAsync(item => item.UserId == userId);

        var itemIds = inventoryItemEntities
            .Select(item => item.CatalogItemId);

        var catalogItemEntities = await catalogItemsRepository
            .GetAllAsync(item => itemIds.Contains(item.Id));

        var inventoryItemDtos = inventoryItemEntities
            .Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities
                .Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);

                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

        return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto grandItemsDto)
    {
        var inventoryItem = await this.inventoryItemsRepository
            .GetAsync(item => item.UserId == grandItemsDto.UserId &&
                      item.CatalogItemId == grandItemsDto.CatalogItemId);

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grandItemsDto.CatalogItemId,
                UserId = grandItemsDto.UserId,
                Quantity = grandItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            };

            await this.inventoryItemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grandItemsDto.Quantity;

            await this.inventoryItemsRepository.UpdateAsync(inventoryItem);
        }

        return Ok();
    }
}
