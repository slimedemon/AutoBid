using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class AuctionDbContext: DbContext
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    {
    }

    public DbSet<Entities.Auction> Auctions { get; set; }
    public DbSet<Entities.Item> Items { get; set; }
}