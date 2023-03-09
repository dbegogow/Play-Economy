namespace Play.Inventory.Service;

public record GrandItemsDto(
        Guid UserId,
        Guid
        CatalogItemId,
        int Quantity);

public record InventoryItemDto(
    Guid CatalogItemId,
    int Quantity,
    DateTimeOffset AcquiredDate);
