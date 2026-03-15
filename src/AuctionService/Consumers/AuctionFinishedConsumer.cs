using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AuctionService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly AuctionDbContext _context;
    private readonly ILogger<AuctionFinishedConsumer> _logger;

    public AuctionFinishedConsumer(AuctionDbContext context, ILogger<AuctionFinishedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        _logger.LogInformation("Consuming AuctionFinished for auction {AuctionId}", context.Message.AuctionId);

        var auction = await _context.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

        if(context.Message.ItemSold)
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount;
        }

        auction.Status = auction.SoldAmount > auction.ReservePrice 
            ? Entities.Status.Finished : Entities.Status.ReserveNotMet;

        await _context.SaveChangesAsync();
    }
}
