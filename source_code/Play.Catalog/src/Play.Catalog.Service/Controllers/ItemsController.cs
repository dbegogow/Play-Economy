using Microsoft.AspNetCore.Mvc;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> items = new List<ItemDto>
    {
        new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison ", 7, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
    };

    [HttpGet]
    public IEnumerable<ItemDto> Get() => items;

    [HttpGet("{id}")]
    public ItemDto GetById(Guid id)
        => items.FirstOrDefault(i => i.Id == id);

    [HttpPost]
    public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
    {
        var item = new ItemDto(
            Guid.NewGuid(),
            createItemDto.Name,
            createItemDto.Description,
            createItemDto.Price,
            DateTimeOffset.UtcNow);

        items.Add(item);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = items
            .FirstOrDefault(i => i.Id == id);

        var updatedItem = existingItem with
        {
            Name = updateItemDto.Name,
            Description = updateItemDto.Description,
            Price = updateItemDto.Price
        };

        var index = items
            .FindIndex(existingItem => existingItem.Id == id);
        items[index] = updatedItem;

        return NoContent();
    }

    [HttpDelete]
    public IActionResult Delete(Guid id)
    {
        var index = items
            .FindIndex(existingItem => existingItem.Id == id);

        items.RemoveAt(index);

        return NoContent();
    }
}
