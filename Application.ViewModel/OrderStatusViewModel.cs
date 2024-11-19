namespace Application.ViewModel
{
    public class OrderStatusViewModel
    {
        public string OrderId { get; set; }
        public int NewStatusId { get; set; }
        public string NewStatus { get; set; }
        public int OldStatusId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal DueAmount { get; set; }
        public bool IsPurchaseOrder { get; set; }
        public string PaymentType { get; set; }
        public decimal TotalDeposited { get; set; }
    }
}
