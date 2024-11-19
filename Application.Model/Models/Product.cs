using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderItems = new List<OrderItem>();
            ProductImages = new List<ProductImage>();
            ProductLowStocks = new List<ProductLowStock>();
            ProductStocks = new List<ProductStock>();
            Purchases = new List<Purchase>();
            PurchaseProductStocks = new List<PurchaseProductStock>();
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public int BranchId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<int> ItemTypeId { get; set; }
        public int Code { get; set; }
        public string ShortCode { get; set; }
        public string Barcode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsFeatured { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> RetailPrice { get; set; }
        public int Quantity { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string Unit { get; set; }
        public Nullable<bool> IsDiscount { get; set; }
        public string DiscountType { get; set; }
        public int? LowStockAlert { get; set; }
        public Nullable<int> ViewCount { get; set; }
        public Nullable<int> SoldCount { get; set; }
        public Nullable<System.DateTime> ExpireDate { get; set; }
        public Nullable<bool> IsInternal { get; set; }
        public Nullable<bool> IsFastMoving { get; set; }
        public Nullable<bool> IsMainItem { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsDeleted { get; set; }
        public Nullable<decimal> Gst { get; set; }
        public string Status { get; set; }
        public System.DateTime? ActionDate { get; set; }
        public Nullable<bool> IsSync { get; set; }
        public string Condition { get; set; }
        public string Color { get; set; }
        public string Capacity { get; set; }
        public string Manufacturer { get; set; }
        public string ModelNumber { get; set; }
        public string WarrantyPeriod { get; set; }
        public Nullable<bool> IsFrozen { get; set; }
        public string IMEI { get; set; }
        public Nullable<decimal> WholesalePrice { get; set; }
        public Nullable<decimal> OnlinePrice { get; set; }
        public Nullable<decimal> RetailDiscount { get; set; }
        public Nullable<decimal> WholesaleDiscount { get; set; }
        public Nullable<decimal> OnlineDiscount { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Category Category { get; set; }
        public virtual ItemType ItemType { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductLowStock> ProductLowStocks { get; set; }
        public virtual User User { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ProductStock> ProductStocks { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<PurchaseProductStock> PurchaseProductStocks { get; set; }
        public int? LastReceivedQuantity { get; set; }
        public string Option { get; set; }
        public bool IsSoldbyweight { get; set; }
    }
}
