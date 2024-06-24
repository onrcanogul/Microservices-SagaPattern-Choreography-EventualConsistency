﻿namespace Order.API.Models.ViewModels
{
    public class CreateOrderVM
    {
        public Guid BuyerId { get; set; }
        public List<OrderItemVM> OrderItems { get; set; }

    }
    public class OrderItemVM
    {
        public Guid ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
