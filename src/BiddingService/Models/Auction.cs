using MongoDB.Entities;

namespace BiddingService.Models;

public class Auction : Entity
{
    public DateTime AuctionEnd { get; set; }
    public string Seller { get; set; }
    public int ReservePrice { get; set; }
    public bool Finished {get; set;}
    public int CurrentHighBid { get; set; } = 0;
    public int Version { get; set; } = 0;

}
