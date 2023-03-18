using Play.Catalog.Service.Entities;
using Play.Catalog.Contracts;
using Play.Common;
using Microsoft.AspNetCore.Mvc;
using MassTransit;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private readonly IRepository<Item> itemsRepository;
    private readonly IPublishEndpoint publishEndpoint;

    public ItemsController(
        IRepository<Item> itemsRepository,
        IPublishEndpoint publishEndpoint)
    {
        this.itemsRepository = itemsRepository;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        => Ok((await this.itemsRepository.GetAllAsync())
            .Select(item => item.AsDto()));

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
        var item = await this.itemsRepository.GetAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return item.AsDto();
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await this.itemsRepository.CreateAsync(item);

        await this.publishEndpoint
            .Publish(new CatalogItemCreated(
                item.Id,
                item.Name,
                item.Description));

        return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await this.itemsRepository.GetAsync(id);

        if (existingItem == null)
        {
            return NotFound();
        }

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await this.itemsRepository.UpdateAsync(existingItem);

        await this.publishEndpoint
            .Publish(new CatalogItemUpdated(
                existingItem.Id,
                existingItem.Name,
                existingItem.Description));

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await this.itemsRepository.GetAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        await this.itemsRepository.RemoveAsync(item.Id);

        await this.publishEndpoint
            .Publish(new CatalogItemDeleted(id));

        return NoContent();
    }
}
