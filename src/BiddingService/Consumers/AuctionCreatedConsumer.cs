using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        var auctionId = context.Message.Id.ToString();
        var existingAuction = await DB.Find<Auction>().OneAsync(auctionId);

        if (existingAuction != null)
        {
            return;
        }

        var auction = new Auction
        {
            ID = auctionId,
            Seller = context.Message.Seller,
            ReservePrice = context.Message.ReservePrice,
            AuctionEnd = context.Message.AuctionEnd
        };

        await auction.SaveAsync();
    }  
}
