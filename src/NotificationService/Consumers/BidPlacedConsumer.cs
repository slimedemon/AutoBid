using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class BidPlacedConsumer: IConsumer<BidPlaced>
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<BidPlacedConsumer> _logger;

    public BidPlacedConsumer(IHubContext<NotificationHub> hubContext, ILogger<BidPlacedConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        _logger.LogInformation("BidPlaced event received in NotificationService for auction {AuctionId}", context.Message.AuctionId);

        await _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}

