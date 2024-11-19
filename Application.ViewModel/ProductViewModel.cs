using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace Application.ViewModel
{
    public class ProductViewModel
    {
        public string Id { get; set; }
        public int Code { get; set; }
        public string Title { get; set; }
        public string Barcode { get; set; }
        public string ShortCode { get; set; }
        public string RootCategoryName { get; set; }
        public int BranchId { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<int> ItemTypeId { get; set; }
        public string SupplierName { get; set; }
        public string ItemTypeName { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string TitleSEO { get; set; }
        public string Description { get; set; }
        public string PrimaryImageName { get; set; }
        public bool IsApproved { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionDateString { get; set; }
        public string PostingTime { get; set; }
        public int Quantity { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> RetailPrice { get; set; }
        public Nullable<decimal> WholesalePrice { get; set; }
        public Nullable<decimal> OnlinePrice { get; set; }
        public Nullable<decimal> RetailDiscount { get; set; }
        public Nullable<decimal> WholesaleDiscount { get; set; }
        public Nullable<decimal> OnlineDiscount { get; set; }
        public string CostPriceText { get; set; }
        public string PriceText { get; set; }
        public string PriceTextOld { get; set; }
        public string DiscountText { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsFeatured { get; set; }
        public bool? IsInternal { get; set; }
        public Nullable<bool> IsFastMoving { get; set; }
        public Nullable<bool> IsMainItem { get; set; }
        public Nullable<bool> IsFrozen { get; set; }
        public decimal? Weight { get; set; }
        public int LowStockAlert { get; set; }
        public string Unit { get; set; }
        public string WeightText { get; set; }
        public string Attributes { get; set; }
        public string Condition { get; set; }
        public string Color { get; set; }
        public string Capacity { get; set; }
        public string Manufacturer { get; set; }
        public string IMEI { get; set; }
        public string ModelNumber { get; set; }
        public string WarrantyPeriod { get; set; }
        public List<BreadCrumbViewModel> BreadCrumbList { get; set; }
        public List<ProductImageViewModel> ImageList { get; set; }
        public UserViewModel Seller { get; set; }
        public CategoryViewModel Category { get; set; }
        public Nullable<decimal> Gst { get; set; }
        public int LastReceivedQuantity { get; set; }
        public List<ColorViewModel> Colors { get; set; }
        public List<SizeViewModel> Sizes { get; set; }
        public List<SizeColorViewModel> SizeColors { get; set; }
        public List<ProductChoiceViewModel> Choices { get; set; }
        public string[] Options { get; set; }
        public bool IsSoldbyweight { get; set; }
    }
}