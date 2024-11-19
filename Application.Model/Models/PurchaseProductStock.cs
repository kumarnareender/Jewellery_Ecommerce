using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class PurchaseProductStock
    {
        public string Id { get; set; }
        public string PurchaseId { get; set; }
        public int SupplierId { get; set; }
        public string ProductId { get; set; }
        public int StockLocationId { get; set; }
        public int Quantity { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public virtual Product Product { get; set; }
        public virtual Purchase Purchase { get; set; }
        public virtual StockLocation StockLocation { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
