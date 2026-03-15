using AuctionService.Data;
using Contracts.Grpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace AuctionService.Services;

public class GrpcAuctionService : GrpcAuction.GrpcAuctionBase
{
    private readonly AuctionDbContext _dbContext;
    private readonly ILogger<GrpcAuctionService> _logger;

    public GrpcAuctionService(AuctionDbContext dbContext, ILogger<GrpcAuctionService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received Grpc request for auction {AuctionId}", request.Id);

        var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id))
            ?? throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));
        
        var response = new GrpcAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                AuctionEnd = auction.AuctionEnd.ToString(),
                Id = auction.Id.ToString(),
                Seller = auction.Seller,
                ReservePrice = auction.ReservePrice,
            }
        };

        return response;
    }
}
