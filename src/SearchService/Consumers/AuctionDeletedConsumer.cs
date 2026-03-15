using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    private readonly ILogger<AuctionDeletedConsumer> _logger;

    public AuctionDeletedConsumer(ILogger<AuctionDeletedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        _logger.LogInformation("Consuming auction deleted: {AuctionId}", context.Message.Id);

        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionDeleted), $"Problem with deleting item from mongodb");
        }
    }
}
