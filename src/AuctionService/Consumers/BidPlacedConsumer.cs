using System;
using AuctionService.Data;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AuctionService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _context;
    private readonly ILogger<BidPlacedConsumer> _logger;

    public BidPlacedConsumer(AuctionDbContext context, ILogger<BidPlacedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        _logger.LogInformation("Consuming BidPlaced for auction {AuctionId}", context.Message.AuctionId);

        var auction = await _context.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

        if (auction.CurrentHighBid == null
            || context.Message.BidStatus.Contains("Accepted")
            && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await _context.SaveChangesAsync();
        }
    }
}
