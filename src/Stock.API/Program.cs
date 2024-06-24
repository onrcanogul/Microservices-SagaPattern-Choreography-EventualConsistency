using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.API.Consumers;
using Stock.API.Models.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StockAPIDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});


builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();
    configure.AddConsumer<PaymentFailedEventConsumer>();
    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
        configurator.ReceiveEndpoint(RabbitMqSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMqSettings.Stock_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});

var app = builder.Build();


using var scope = app.Services.CreateScope();
StockAPIDbContext context = scope.ServiceProvider.GetRequiredService<StockAPIDbContext>();

if (!context.Stocks.Any())
{
    context.Stocks.Add(new()
    {
        Count = 200,
        ProductId = Guid.NewGuid()
    });
    context.Stocks.Add(new()
    {
        Count = 200,
        ProductId = Guid.NewGuid()
    });
    context.Stocks.Add(new()
    {
        Count = 200,
        ProductId = Guid.NewGuid()
    });
    context.Stocks.Add(new()
    {
        Count = 200,
        ProductId = Guid.NewGuid()
    });


    await context.SaveChangesAsync();
}



app.Run();
