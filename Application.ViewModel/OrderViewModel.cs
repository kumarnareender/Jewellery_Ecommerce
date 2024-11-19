using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.ViewModel
{
    public class OrderViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public long OrderCode { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<decimal> PayAmount { get; set; }
        public Nullable<decimal> DueAmount { get; set; }
        public Nullable<decimal> ReceiveAmount { get; set; }
        public Nullable<decimal> ChangeAmount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<decimal> ShippingAmount { get; set; }
        public Nullable<decimal> ItemTotal { get; set; }
        public Nullable<decimal> TotalCostPrice { get; set; }
        public Nullable<decimal> TotalProfit { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public bool IsFrozen { get; set; }
        public string OrderMode { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public Nullable<System.DateTime> ActionDate { get; set; }
        public string ActionDateString { get; set; }
        public string ActionBy { get; set; }
        public virtual List<OrderItemViewModel> OrderItems { get; set; }
        public string OrderType { get; set; }
        public int StatusId { get; set; }
        public int PaymentStatusId { get; set; }

        public string Payment { get; set; }
        public decimal AmountDeposited { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TableNumber { get; set; }
        public string Notes { get; set; }
        public string CustomerId { get; set; }
    }

    public class OrderItemViewModel
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public decimal? Discount { get; set; }
        public System.DateTime ActionDate { get; set; }
        public int ReceivedQuantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public string Options { get; set; }
        public bool Printed { get; set; }
        public decimal Weight { get; set; }
        public string Unit { get; set; }
    }
}