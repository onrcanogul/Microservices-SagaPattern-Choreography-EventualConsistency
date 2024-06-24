# Microservices-Eventual Consistency-Saga Pattern
The saga choreography pattern helps preserve data integrity in distributed transactions that span multiple services by using event subscriptions. In a distributed transaction, multiple services can be called before a transaction is completed. When the services store data in different data stores, it can be challenging to maintain data consistency across these data stores.

We are created dummy data for Stock.API. We used this data as a client.

Events:

Order -> OrderCreatedEvent --> Consumer : Stock.API

Stock -> StockReservedEvent --> Consumer : Payment.API
      -> StockNotReservedEvent --> Consumer : Order.API

Payment -> PaymentCompletedEvent --> Consumer : Order.API
        -> PaymentFailedEvent --> Consumer : Order.API
                              --> Consumer : Stock.API(update stock(transaction))

We triggered the PaymentCompeleted event or PaymentFailedEvent by simply simulating the payment in Payment.API

PaymentCompleted(result is true in StockReservedEventConsumer) :
![WhatsApp Görsel 2024-06-24 saat 15 33 04_94f7f872](https://github.com/onrcanogul/sagapattern-choreography-eventualconsistency/assets/147406204/17a28352-26a0-40e6-819d-171c99603b8d)
![WhatsApp Görsel 2024-06-24 saat 15 33 34_4d9b2663](https://github.com/onrcanogul/sagapattern-choreography-eventualconsistency/assets/147406204/beae29aa-977f-490d-95fe-b5881837cd68)
![WhatsApp Görsel 2024-06-24 saat 15 33 54_877092b4](https://github.com/onrcanogul/sagapattern-choreography-eventualconsistency/assets/147406204/33529587-aabd-4f20-b5b3-b67d384f55a5)

Dummy data:
```
{
  "buyerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "orderItems": [
    {
      "productId": "c8ed77e6-a867-4601-b841-321a9aa93470",
      "count": 10,
      "price": 20
    },
    {
      "productId": "68a65c1a-5003-4f8a-ac27-5c08904309fe",
      "count": 20,
      "price": 10
    },
    {
      "productId": "47753d67-8695-40a6-97e8-cca41388960d",
      "count": 30,
      "price": 40
    },
    {
      "productId": "b78d8b29-ac1b-4a92-bfaf-e86dc2517ea8",
      "count": 50,
      "price": 10
    }
  ]
}
```


PaymentFailed(result is false in StockReservedEventConsumer) :
![WhatsApp Görsel 2024-06-24 saat 15 33 04_010f73d9](https://github.com/onrcanogul/sagapattern-choreography-eventualconsistency/assets/147406204/d0923465-9d1c-4aaa-b789-d0ea91d005a5)
![WhatsApp Görsel 2024-06-24 saat 15 40 27_91d39e36](https://github.com/onrcanogul/sagapattern-choreography-eventualconsistency/assets/147406204/b4436a3d-ad97-4f3b-b1a2-ef47600ce723)
![WhatsApp Görsel 2024-06-24 saat 15 40 52_2d641feb](https://github.com/onrcanogul/sagapattern-choreography-eventualconsistency/assets/147406204/2fe42037-b947-435e-adb1-c3241503b9af)




