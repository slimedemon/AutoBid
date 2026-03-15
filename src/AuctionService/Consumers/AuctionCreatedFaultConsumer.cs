using Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AuctionService.Consumers;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    private readonly ILogger<AuctionCreatedFaultConsumer> _logger;

    public AuctionCreatedFaultConsumer(ILogger<AuctionCreatedFaultConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        _logger.LogInformation("Consuming faulty auction creation");

        var exception = context.Message.Exceptions.First();

        if (exception != null && exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Model = "Foobar";
            await context.Publish(context.Message.Message);
        }
        else
        {
            _logger.LogWarning("Fault is not recoverable: {ExceptionType}", exception?.ExceptionType);
        }

    }
}
