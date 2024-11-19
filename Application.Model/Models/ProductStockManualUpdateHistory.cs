using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class ProductStockManualUpdateHistory
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int StockLocationId { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
        public System.DateTime ActionDate { get; set; }
        public string ActionBy { get; set; }
    }
}
