using System;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<List<Item>> GetItemsAsync()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(i => i.UpdatedAt, Order.Descending)
            .Project(i => i.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        var items = await _httpClient
            .GetFromJsonAsync<List<Item>>(
                $"{_config["AuctionServiceUrl"] ?? "http://localhost:7001"}/api/auctions?updatedAfter={lastUpdated ?? string.Empty}"
            );
        
        return items ?? new List<Item>();
    }
}
