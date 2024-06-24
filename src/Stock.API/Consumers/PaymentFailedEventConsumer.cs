using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.API.Models.Contexts;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer(StockAPIDbContext dbContext) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await dbContext.Stocks.FirstOrDefaultAsync(s => s.ProductId == orderItem.ProductId);
                stock.Count += orderItem.Count;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
