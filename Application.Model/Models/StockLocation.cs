using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class StockLocation
    {
        public StockLocation()
        {
            this.ProductStocks = new List<ProductStock>();
            this.PurchaseProductStocks = new List<PurchaseProductStock>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ProductStock> ProductStocks { get; set; }
        public virtual ICollection<PurchaseProductStock> PurchaseProductStocks { get; set; }
    }
}
