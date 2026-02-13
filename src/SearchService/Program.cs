using Contracts;
using MassTransit;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.RequestHelper;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(HttpPollyHelper.GetAsyncPolicy());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, configure) =>
    {
        configure.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
    
        configure.ReceiveEndpoint("search-auction-updated", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
        });

        configure.ReceiveEndpoint("search-auction-deleted", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<AuctionDeletedConsumer>(context);
        });

        configure.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitializeAsync(app);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }  
});

app.Run();