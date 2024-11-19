using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class ProductStockViewModel
    {
        public int StockLocationId { get; set; }
        public string StockLocationName { get; set; }
        public int CurrentQuantity { get; set; }
        public int Quantity { get; set; }
    }
}
