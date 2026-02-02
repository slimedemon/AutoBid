using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    private readonly IMapper _mapper;

    public AuctionUpdatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> Consuming auction updated: " + context.Message.Id);

        var update = DB.Update<Item>()
            .Match(i => i.ID == context.Message.Id);

        if (!string.IsNullOrEmpty(context.Message.Model))
            update = update.Modify(i => i.Model, context.Message.Model);

        if (!string.IsNullOrEmpty(context.Message.Make))
            update = update.Modify(i => i.Make, context.Message.Make);

        if (!string.IsNullOrEmpty(context.Message.Color))
            update = update.Modify(i => i.Color, context.Message.Color);

        if (context.Message.Mileage != null && context.Message.Mileage > 0)
            update = update.Modify(i => i.Mileage, context.Message.Mileage);

        if (context.Message.Year != null && context.Message.Year > 0)
            update = update.Modify(i => i.Year, context.Message.Year);

        await update.ExecuteAsync();
    }
}
