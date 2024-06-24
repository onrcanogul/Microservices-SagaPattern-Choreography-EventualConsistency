using MassTransit;
using Shared;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer(ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint) : IConsumer<StockReservedEvent>
    {
        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            //payment stuff...
            bool result = false;
            if (result)
            {
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                };
                ISendEndpoint sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new($"queue:{RabbitMqSettings.Order_PaymentCompletedEventQueue}"));
                await sendEndpoint.Send(paymentCompletedEvent);

                Console.WriteLine("Payment is completed");
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "payment failed",
                    OrderItems = context.Message.OrderItems
                };

                await publishEndpoint.Publish(paymentFailedEvent);

                Console.WriteLine("Payment is failed");
            }

        }
    }
}
