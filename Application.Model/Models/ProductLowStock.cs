using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class ProductLowStock
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public Nullable<int> LowStockQuantity { get; set; }
        public Nullable<System.DateTime> LowStockEntryDate { get; set; }
        public Nullable<bool> IsOrder { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public string AddedBy { get; set; }
        public string UpdateBy { get; set; }
        public virtual Product Product { get; set; }
    }
}
