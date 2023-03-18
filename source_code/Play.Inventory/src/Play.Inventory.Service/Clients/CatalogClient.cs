namespace Play.Inventory.Service.Clients;

public class CatalogClient
{
    private readonly HttpClient httpClient;

    public CatalogClient(HttpClient httpClient)
        => this.httpClient = httpClient;

    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsyn()
        => await this.httpClient
            .GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
}
