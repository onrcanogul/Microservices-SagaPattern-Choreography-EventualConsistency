using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models.Contexts;
using Shared.Events;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer(OrderAPIDbContext dbContext) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Models.Order? order = await dbContext.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);
            if(order is not null)
            {
                order.OrderStatus = Models.Enums.OrderStatus.Failed;
                await dbContext.SaveChangesAsync();
            }
            else 
                throw new ArgumentNullException();

            
        }
    }
}
