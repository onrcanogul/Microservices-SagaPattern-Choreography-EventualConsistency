using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Events;
using Stock.API.Models.Contexts;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(StockAPIDbContext dbContext, ISendEndpointProvider sendEndpointProvider) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {

            List<bool> results = new();
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stockResult = await dbContext.Stocks.AnyAsync(x => x.ProductId == orderItem.ProductId && x.Count >= orderItem.Count);
                results.Add(stockResult);
            }
            if (results.TrueForAll(e => e.Equals(true)))
            {
                //update stock
                foreach (var orderItem in context.Message.OrderItems)
                {
                    var stock = await dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == orderItem.ProductId);

                    stock.Count -= orderItem.Count;
                    await dbContext.SaveChangesAsync();
                }

                //publish event
                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems
                };

                ISendEndpoint sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new($"queue:{RabbitMqSettings.Payment_StockReservedEventQueue}"));
                await sendEndpoint.Send(stockReservedEvent); 
                
            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Stock not enough"
                };

                ISendEndpoint sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new($"queue:{RabbitMqSettings.Order_StockNotReservedEventQueue}"));
                await sendEndpoint.Send(stockNotReservedEvent);
            }
        }
    }
}
