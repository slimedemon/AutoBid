using MassTransit;
using NotificationService.Consumers;
using NotificationService.Exceptions;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

    x.UsingRabbitMq((context, configure) =>
    {
        configure.Host(builder.Configuration["RabbitMq:Host"], "/", h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            h.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });

        configure.ConfigureEndpoints(context);
    });
});
builder.Services.AddSignalR();

var app = builder.Build();

app.UseExceptionHandler();

app.MapHub<NotificationHub>("/notifications");

app.Run();
