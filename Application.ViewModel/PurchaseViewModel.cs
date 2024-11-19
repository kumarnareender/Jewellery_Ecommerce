using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel
{
    public class PurchaseViewModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int SupplierId { get; set; }
        public string InvoiceNo { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public string StockLocation { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> TaxPerc { get; set; }
        public Nullable<decimal> TaxAmount { get; set; }        
        public decimal UnitPrice { get; set; }
        public decimal TotalPriceExcTax { get; set; }
        public decimal TotalPriceIncTax { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public string PurchaseDateString { get; set; }
        public System.DateTime ActionDate { get; set; }
        public List<ProductStockViewModel> StockList { get; set; }
    }
}
