using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<AuctionFinishedConsumer> _logger;

    public AuctionFinishedConsumer(IHubContext<NotificationHub> hubContext, ILogger<AuctionFinishedConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        _logger.LogInformation("AuctionFinished event received in NotificationService for auction {AuctionId}", context.Message.AuctionId);

        await _hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
    }
}
