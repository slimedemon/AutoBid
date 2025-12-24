using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public class DbInitializer
{
    public static async Task InitializeAsync(WebApplication app)
    {
        var mongoConnectionString = app.Configuration.GetConnectionString("MongoDbConnection");
        var mongoSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
        await DB.InitAsync("SearchDb", mongoSettings);

        await DB.Index<Item>()
            .Key(i => i.Make, KeyType.Text)
            .Key(i => i.Model, KeyType.Text)
            .Key(i => i.Color, KeyType.Text)
            .CreateAsync();

        var count = await DB.CountAsync<Item>();

        // Seed initial data if needed
        if (count == 0)
        {
            var scope = app.Services.CreateScope();
            var auctionClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
            var items = await auctionClient.GetItemsAsync();

            await DB.SaveAsync(items);
        }            
    }
}
