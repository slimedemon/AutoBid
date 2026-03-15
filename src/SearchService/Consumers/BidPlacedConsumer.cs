using System;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly ILogger<BidPlacedConsumer> _logger;

    public BidPlacedConsumer(ILogger<BidPlacedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        _logger.LogInformation("Consuming BidPlaced for auction {AuctionId}", context.Message.AuctionId);

        var auction = await DB.Find<Item>()
            .OneAsync(context.Message.AuctionId);

        if(auction.CurrentHighBid == null 
            || context.Message.BidStatus.Contains("Accepted")
            && context.Message.Amount > auction.CurrentHighBid)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await auction.SaveAsync();
        }
    }
}
