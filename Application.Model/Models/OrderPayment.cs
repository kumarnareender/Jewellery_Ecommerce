using System;

namespace Application.Model.Models
{
    public partial class OrderPayment
    {
        //public long Id { get; set; }
        //public long OrderId { get; set; }
        //public Order Order { get; set; }
        //public string PaymentType { get; set; }
        //public decimal Amount { get; set; }
        //public string Reference { get; set; }
        public long Id { get; set; }
        public string OrderId { get; set; }
        public int OldStatusId { get; set; }
        public int NewStatusId { get; set; }
        public string PaymentType { get; set; }
        public string Description { get; set; }
        public decimal AmountDeposited { get; set; }
        public decimal DueAmount { get; set; }
        public decimal TotalDeposited { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public string Payment { get; set; }
        public Nullable<bool> IsPurchaseOrder { get; set; }

    }
}
