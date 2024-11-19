using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class Purchase
    {
        public Purchase()
        {
            this.PurchaseProductStocks = new List<PurchaseProductStock>();
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public int SupplierId { get; set; }
        public string InvoiceNo { get; set; }
        public int Quantity { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public Nullable<decimal> TaxPerc { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }
        public string Remarks { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public System.DateTime ActionDate { get; set; }
        public string ActionBy { get; set; }
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<PurchaseProductStock> PurchaseProductStocks { get; set; }
    }
}
