using Play.Common;
using Play.Inventory.Service.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Play.Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<InventoryItem> itemsRepository;

    public ItemsController(IRepository<InventoryItem> itemsRepository)
        => this.itemsRepository = itemsRepository;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest();
        }

        var items = (await this.itemsRepository.GetAllAsync(item => item.UserId == userId))
                    .Select(item => item.AsDto());

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(GrandItemsDto grandItemsDto)
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
