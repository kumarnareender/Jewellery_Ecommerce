using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class ProductStock
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int StockLocationId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string Unit { get; set; }
        public virtual Product Product { get; set; }
        public virtual StockLocation StockLocation { get; set; }
    }
}
