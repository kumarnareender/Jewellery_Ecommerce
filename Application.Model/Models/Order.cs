using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public long OrderCode { get; set; }
        public string Barcode { get; set; }
        public Nullable<decimal> PayAmount { get; set; }
        public Nullable<decimal> DueAmount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<decimal> ShippingAmount { get; set; }
        public Nullable<decimal> ReceiveAmount { get; set; }
        public Nullable<decimal> ChangeAmount { get; set; }
        public string OrderMode { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }
        public Nullable<System.DateTime> ActionDate { get; set; }
        public string ActionBy { get; set; }
        public Nullable<int> BranchId { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
        public Nullable<bool> IsFrozen { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual User User { get; set; }
        public string OrderType { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<bool> IsWholeSaleOrder { get; set; }
        //public string CustomerId { get; set; }
        public string CustomerId { get; set; }

        public int? PaymentStatusId { get; set; }
        public Nullable<int> TableNumber { get; set; }
        public string Notes { get; set; }
    }
}
