using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> itemsRepository;
    private readonly CatalogClient catalogClient;

    public ItemsController(
        IRepository<InventoryItem> itemsRepository,
        CatalogClient catalogClient)
    {
        this.itemsRepository = itemsRepository;
        this.catalogClient = catalogClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var catalogItems = await catalogClient.GetCatalogItemsAsyn();
        var inventoryItemEntities = await itemsRepository
            .GetAllAsync(item => item.UserId == userId);

        var inventoryItemDtos = inventoryItemEntities
            .Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

        return Ok(inventoryItemDtos);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto grandItemsDto)
    {
        var inventoryItem = await this.itemsRepository
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

            await this.itemsRepository.CreateAsync(inventoryItem);
        }
        else
        {
            inventoryItem.Quantity += grandItemsDto.Quantity;

            await this.itemsRepository.UpdateAsync(inventoryItem);
        }

        return Ok();
    }
}
