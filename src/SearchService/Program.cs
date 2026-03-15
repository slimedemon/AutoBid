using Contracts;
using MassTransit;
using Polly;
using SearchService.Exceptions;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.RequestHelper;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(HttpPollyHelper.GetAsyncPolicy());

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, configure) =>
    {
        configure.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));

        configure.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            h.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });

        configure.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    await Policy.Handle<TimeoutException>()
        .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(10))
        .ExecuteAndCaptureAsync(async () =>
        {
            await DbInitializer.InitializeAsync(app);
        });
});

app.Run();