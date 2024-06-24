using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Order.API.Models.Contexts;
using Order.API.Models.ViewModels;
using Shared;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderAPIDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<PaymentFailedEventConsumer>();
    configure.AddConsumer<PaymentCompletedEventConsumer>();
    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration["RabbitMQ"]);
        configurator.ReceiveEndpoint(RabbitMqSettings.Order_PaymentFailedEventQueue, e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
        configurator.ReceiveEndpoint(RabbitMqSettings.Order_PaymentCompletedEventQueue, e => e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
    });
});

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();


app.MapPost("/create-order", async ([FromBody]CreateOrderVM model,OrderAPIDbContext context,IPublishEndpoint publishEndpoint) =>
{
    Order.API.Models.Order order = new Order.API.Models.Order()
    {
        BuyerId = model.BuyerId,
        OrderStatus = Order.API.Models.Enums.OrderStatus.Pending,
        OrderItems = model.OrderItems.Select(x => new OrderItem
        {
            ProductId = x.ProductId,
            Price = x.Price,
            Count = x.Count,
        }).ToList(),
        Price = model.OrderItems.Sum(x=> x.Price * x.Count),
    };
    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();


    OrderCreatedEvent orderCreatedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        OrderItems = order.OrderItems.Select(x => new Shared.Messages.OrderItemMessage
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId
        }).ToList(),
        TotalPrice = order.Price
    };

    await publishEndpoint.Publish(orderCreatedEvent);

});

app.UseHttpsRedirection();


app.Run();
