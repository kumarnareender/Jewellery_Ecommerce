using Application.Common;
using Application.Logging;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    [Authorize(Roles = "admin,manager,salesperson")]
    public class AdminController : Controller
    {
        private IUserService userService;
        private IProductService productService;
        private IProductImageService productImageService;
        private ISupplierService supplierService;
        private readonly IActionLogService actionLogService;

        public AdminController(IUserService userService, IProductService productService, IProductImageService productImageService, ISupplierService supplierService, IActionLogService actionLogService)
        {
            this.userService = userService;
            this.productService = productService;
            this.productImageService = productImageService;
            this.supplierService = supplierService;
            this.actionLogService = actionLogService;
        }

        public ActionResult Index()
        {
            return View("Dashboard");
        }

        public ActionResult Category()
        {
            return View();
        }

        public ActionResult CustomerAdd()
        {
            return View();
        }

        public ActionResult CustomerList()
        {
            return View();
        }

        public ActionResult ProductList()
        {
            return View();
        }

        public ActionResult PhoneOrder()
        {
            return View();
        }

        public JsonResult GetWholesaleCustomerList()
        {
            List<UserViewModel> itemList = new List<UserViewModel>();

            IEnumerable<User> list = userService.GetAllCustomers().Where(x => x.IsWholeSaler);
            foreach (User user in list)
            {
                UserViewModel item = new UserViewModel
                {
                    Id = user.Id,
                    CustomerCode = user.CustomerCode,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Password = user.Password,
                    ShipCountry = user.ShipCountry,
                    ShipZipCode = user.ShipZipCode,
                    ShipAddress = user.ShipAddress,
                    ShipCity = user.ShipCity,
                    ShipState = user.ShipState,
                    Mobile = user.Mobile,
                    Email = user.Email
                };

                itemList.Add(item);
            }

            return new JsonResult()
            {
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json",
                Data = itemList,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }


        public JsonResult GetCustomerList()
        {
            List<UserViewModel> itemList = new List<UserViewModel>();

            IEnumerable<User> list = userService.GetAllCustomers();
            foreach (User user in list)
            {
                UserViewModel item = new UserViewModel
                {
                    Id = user.Id,
                    CustomerCode = user.CustomerCode,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Password = user.Password,
                    ShipCountry = user.ShipCountry,
                    ShipZipCode = user.ShipZipCode,
                    ShipAddress = user.ShipAddress,
                    ShipCity = user.ShipCity,
                    ShipState = user.ShipState,
                    Mobile = user.Mobile,
                    Email = user.Email
                };

                itemList.Add(item);
            }

            return new JsonResult()
            {
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json",
                Data = itemList,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        public JsonResult GetCustomer(string id)
        {
            UserViewModel item = new UserViewModel();
            User user = userService.GetUserById(id);

            if (user != null)
            {
                item.Id = user.Id;
                item.FirstName = user.FirstName;
                item.LastName = user.LastName;
                item.Username = user.Username;
                item.Password = user.Password;
                item.ShipCountry = user.ShipCountry;
                item.ShipZipCode = user.ShipZipCode;
                item.ShipAddress = user.ShipAddress;
                item.ShipCity = user.ShipCity;
                item.ShipState = user.ShipState;
                item.Mobile = user.Mobile;
                item.Email = user.Email;
                item.IsWholeSaler = user.IsWholeSaler ? 1 : 0;
            }

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLowStockCount()
        {
            int count = productService.GetLowStockItemCount();
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAdminProductList(int pageNo = 1, string searchText = "", int branchId = 0, int categoryId = 0, int itemTypeId = 0, int supplierId = 0, string attribute = "", string lowStock = "")
        {
            string userId = Utils.GetLoggedInUserId();
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix() + "/Small/";
            List<ProductViewModel> productVMList = new List<ProductViewModel>();

            string allChildCategoryIds = string.Empty;
            if (categoryId > 0)
            {
                allChildCategoryIds = AppCommon.GetAllChildIds11(categoryId.ToString());
            }


            int pageNumber = pageNo;
            int pageSize = 20;

            int recordFrom = (pageNumber - 1) * pageSize + 1;
            int recordTo = recordFrom + (pageSize - 1);

            // Sort order
            string orderBy = " ActionDate desc ";

            // Select clause
            string selectRecords = string.Format(@"
                                            SELECT * FROM 
                                            (
                                                SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNum, p.* FROM Products p
                                                #WHERE#
                                            ) AS TBL ", orderBy);

            // Select total count
            string selectTotalCount = string.Format(@"SELECT COUNT(*) AS TotalRecords FROM Products p ");

            // Where clause
            string whereQuery = string.Format(@" WHERE IsDeleted = 0 ");

            if (!string.IsNullOrEmpty(searchText))
            {
                whereQuery += string.Format(" and (Title Like '%{0}%' or Barcode = '{0}' or IMEI = '{0}') ", searchText);
            }

            if (branchId > 0)
            {
                whereQuery += string.Format(" and BranchId = {0} ", branchId);
            }

            if (!string.IsNullOrEmpty(allChildCategoryIds))
            {
                whereQuery += " and CategoryId IN (" + allChildCategoryIds + ")";
            }

            if (itemTypeId > 0)
            {
                whereQuery += string.Format(" and ItemTypeId = {0} ", itemTypeId);
            }

            if (supplierId > 0)
            {
                whereQuery += string.Format(" and SupplierId = {0} ", supplierId);
            }

            if (!string.IsNullOrEmpty(attribute))
            {
                if (attribute == "Main")
                {
                    whereQuery += " and IsMainItem = 1 ";
                }
                else if (attribute == "FastMoving")
                {
                    whereQuery += " and IsFastMoving = 1 ";
                }
                else if (attribute == "Internal")
                {
                    whereQuery += " and IsInternal = 1 ";
                }
            }

            if (!string.IsNullOrEmpty(lowStock))
            {
                whereQuery += string.Format(" and Quantity <= LowStockAlert ");
            }

            // Exclude anonymous product
            whereQuery += string.Format(" and Id != '00000000-0000-0000-0000-000000000000' ");

            // Paging clause
            string pagingQuery = string.Format(@" WHERE RowNum BETWEEN {0} AND {1}", recordFrom, recordTo);

            string sqlQuery = selectRecords.Replace("#WHERE#", whereQuery) + pagingQuery;
            string sqlTotalRecords = selectTotalCount + whereQuery;

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<ProductViewModel> productList = context.Database.SqlQuery<ProductViewModel>(sqlQuery).ToList();
                if (productList != null && productList.Count > 0)
                {
                    foreach (ProductViewModel product in productList)
                    {
                        ProductViewModel productVM = new ProductViewModel();

                        string priceText = "R: " + Utils.GetCurrencyCode() + string.Format("{0:#,0.00}", product.RetailPrice)
                            + "<br/>" + "w: " + Utils.GetCurrencyCode() + string.Format("{0:#,0.00}", product.WholesalePrice)
                            + "<br/>" + "O: " + Utils.GetCurrencyCode() + string.Format("{0:#,0.00}", product.OnlinePrice);


                        string retailDiscount = product.RetailDiscount != null ? Utils.GetCurrencyCode() + string.Format("{0:#,0.00}", product.RetailDiscount) : Utils.GetCurrencyCode() + "0";
                        string wholesaleDiscount = product.WholesaleDiscount != null ? Utils.GetCurrencyCode() + string.Format("{0:#,0.00}", product.WholesaleDiscount) : Utils.GetCurrencyCode() + "0";
                        string onlineDiscount = product.OnlineDiscount != null ? Utils.GetCurrencyCode() + string.Format("{0:#,0.00}", product.OnlineDiscount) : Utils.GetCurrencyCode() + "0";

                        string discountText = "R: " + retailDiscount
                            + "<br/>" + "w: " + wholesaleDiscount
                            + "<br/>" + "O: " + onlineDiscount;

                        productVM.Id = product.Id;
                        productVM.Title = product.Title;
                        productVM.Barcode = product.Barcode;
                        productVM.ActionDate = product.ActionDate;
                        productVM.ActionDateString = product.ActionDate.ToString();
                        productVM.IsApproved = product.IsApproved;
                        productVM.Status = product.Status;
                        productVM.Quantity = product.Quantity;
                        productVM.ExpireDate = product.ExpireDate;
                        productVM.SupplierName = product.SupplierName;
                        productVM.ItemTypeName = product.ItemTypeName;
                        productVM.PriceText = priceText;
                        productVM.CostPriceText = Utils.GetCurrencyCode() + string.Format("{0:#,0.00}", product.CostPrice);

                        productVM.DiscountText = discountText;
                        productVM.RetailDiscount = product.RetailDiscount == null ? 0 : product.RetailDiscount;
                        productVM.WholesaleDiscount = product.WholesaleDiscount == null ? 0 : product.WholesaleDiscount;
                        productVM.OnlineDiscount = product.OnlineDiscount == null ? 0 : product.OnlineDiscount;

                        productVM.WeightText = product.Weight != null ? (decimal)product.Weight + " " + product.Unit : "";
                        productVM.LowStockAlert = product.LowStockAlert;
                        productVM.ItemTypeName = product.ItemTypeName;

                        // Electronics items
                        productVM.Color = product.Color;
                        productVM.Capacity = product.Capacity;
                        productVM.Condition = product.Condition;
                        productVM.IMEI = product.IMEI;
                        productVM.ModelNumber = product.ModelNumber;
                        productVM.Manufacturer = product.Manufacturer;
                        productVM.WarrantyPeriod = product.WarrantyPeriod;
                        productVM.WholesalePrice = product.WholesalePrice;
                        productVM.WholesaleDiscount = product.WholesaleDiscount;
                        productVM.Gst = product.Gst;
                        productVM.CostPrice = product.CostPrice;
                        productVM.LastReceivedQuantity = product.LastReceivedQuantity;

                        string attributes = string.Empty;
                        if (product.IsMainItem != null && (bool)product.IsMainItem)
                        {
                            attributes += "Main item, ";
                        }
                        if (product.IsFastMoving != null && (bool)product.IsFastMoving)
                        {
                            attributes += "Fast moving, ";
                        }
                        if (product.IsInternal != null && (bool)product.IsInternal)
                        {
                            attributes += "Internal item, ";
                        }
                        if (product.IsFeatured != null && (bool)product.IsFeatured)
                        {
                            attributes += "Show home page, ";
                        }

                        attributes = !string.IsNullOrEmpty(attributes) ? attributes.TrimEnd(' ').TrimEnd(',') : "";
                        productVM.Attributes = attributes;

                        // Get the supplier name if any
                        if (product.SupplierId != null)
                        {
                            Supplier supplier = supplierService.GetSupplier((int)product.SupplierId);
                            if (supplier != null)
                            {
                                productVM.SupplierName = supplier.Name;
                            }
                        }

                        // Get user images
                        List<ProductImageViewModel> imageVMList = new List<ProductImageViewModel>();
                        IEnumerable<ProductImage> imageList = productImageService.GetProductImages(product.Id, true);
                        foreach (ProductImage image in imageList)
                        {
                            if (image.IsPrimaryImage)
                            {
                                productVM.PrimaryImageName = imageSrcPrefix + image.ImageName;
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(productVM.PrimaryImageName))
                        {
                            productVM.PrimaryImageName = imageSrcPrefix + "no-image.jpg";
                        }

                        productVMList.Add(productVM);
                    }
                }

                // Get total records
                int totalPages = 0;
                int totalRecords = 0;

                TotalRecordsViewModel item = context.Database.SqlQuery<TotalRecordsViewModel>(sqlTotalRecords).FirstOrDefault();
                totalPages = (int)Math.Ceiling((double)item.TotalRecords / pageSize);
                totalRecords = item.TotalRecords;

                return Json(new
                {
                    recordList = productVMList,
                    totalPages = totalPages,
                    totalRecords = totalRecords
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetStockList(int branchId, int categoryId = 0)
        {
            string userId = Utils.GetLoggedInUserId();
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix() + "/Small/";
            List<ProductViewModel> productVMList = new List<ProductViewModel>();

            if (!string.IsNullOrEmpty(userId))
            {
                IEnumerable<Product> productList = productService.GetAllProducts(branchId);
                foreach (Product product in productList)
                {
                    ProductViewModel productVM = new ProductViewModel
                    {
                        Id = product.Id,
                        Title = product.Title,
                        Barcode = product.Barcode,
                        Quantity = product.Quantity,
                        ExpireDate = product.ExpireDate,
                        LowStockAlert = product.LowStockAlert.Value
                    };

                    // Get user images
                    List<ProductImageViewModel> imageVMList = new List<ProductImageViewModel>();
                    IEnumerable<ProductImage> imageList = productImageService.GetProductImages(product.Id, true);
                    foreach (ProductImage image in imageList)
                    {
                        if (image.IsPrimaryImage)
                        {
                            productVM.PrimaryImageName = imageSrcPrefix + image.ImageName;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(productVM.PrimaryImageName))
                    {
                        productVM.PrimaryImageName = imageSrcPrefix + "no-image.jpg";
                    }

                    productVMList.Add(productVM);
                }
            }

            return new JsonResult()
            {
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/json",
                Data = productVMList,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = int.MaxValue
            };
        }

        public JsonResult UpdateStock(string productId, int quantity)
        {
            bool isSuccess = true;
            try
            {
                productService.UpdateStockQty(productId, quantity);

                AppCommon.WriteActionLog(actionLogService, "Product", "Update Stock", "Product Id: " + productId + " Stock: " + quantity, "Update", User.Identity.Name);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError(ex, "Failed to update quantity!");
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProduct(string productId)
        {
            bool isSuccess = true;
            try
            {
                isSuccess = productService.DeleteProduct(productId);

                AppCommon.WriteActionLog(actionLogService, "Product", "Delete Product", "Product Id: " + productId, "Delete", User.Identity.Name);

                if (isSuccess)
                {
                    IEnumerable<ProductImage> productImageList = productImageService.GetProductImages(productId, false);
                    foreach (ProductImage pi in productImageList)
                    {
                        AppUtils.DeleteProductImage(productImageService, pi.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError(ex, "Failed to delete user!");
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderStatus()
        {

            string fromDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
            string toDate = DateTime.Now.AddDays(1).Year.ToString() + "-" + DateTime.Now.AddDays(1).Month.ToString() + "-" + DateTime.Now.AddDays(1).Day.ToString();

            string sqlQuery = string.Format(@"SELECT (select count(*) from orders where OrderMode = 'Store' and ActionDate between '{0}' and '{1}') as StoreSell,
                                            (select count(*) from orders where OrderMode = 'Online' and ActionDate between '{0}' and '{1}') as OnlineSell,
                                            (select count(*) from orders where OrderMode = 'PhoneOrder' and ActionDate between '{0}' and '{1}') as PhoneOrderSell,
                                            (select count(*) from orders where OrderStatus = 'Pending' and ActionDate between '{0}' and '{1}') as OrderPending,
                                            (select count(*) from orders where OrderType = 'Delivery' and ActionDate between '{0}' and '{1}') as DeliveryOrders,
                                            (select count(*) from orders where OrderType = 'Pickup' and ActionDate between '{0}' and '{1}') as PickupOrders,
                                            (select Sum(PayAmount) from orders where OrderType = 'Delivery' and ActionDate between '{0}' and '{1}') as DeliveryOrdersSell,
                                            (select Sum(PayAmount) from orders where OrderType = 'Pickup' and ActionDate between '{0}' and '{1}') as PickupOrdersSell,
                                            (select Sum(PayAmount) from orders where ActionDate between '{0}' and '{1}') as TotalAmount,
                                            (select Sum(PayAmount) from orders where PaymentType = 'COD' and ActionDate between '{0}' and '{1}') as CashAmount,
                                            (select Sum(PayAmount) from orders where PaymentType in ('Online', 'Card') and ActionDate between '{0}' and '{1}') as OnlineAmount", fromDate, toDate);

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<OrderStatusViewModel> recordList = context.Database.SqlQuery<OrderStatusViewModel>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    return Json(recordList, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        public JsonResult GetTotalItemValues()
        {

            string sqlQuery = string.Format(@"SELECT (select count(*) from products where IsDeleted = 0) as TotalItemPosted,
                                            (select SUM(CostPrice * Quantity) from products where IsDeleted = 0) as TotalItemValue");

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<TotalItemValues> recordList = context.Database.SqlQuery<TotalItemValues>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    return Json(recordList, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }
    }

    public class TotalRecordsViewModel
    {
        public int TotalRecords { get; set; }
    }

    public class OrderStatusViewModel
    {
        public int StoreSell { get; set; }
        public int OnlineSell { get; set; }
        public int PhoneOrderSell { get; set; }
        public int OrderPending { get; set; }
        public int DeliveryOrders { get; set; }
        public int PickupOrders { get; set; }
        public decimal? DeliveryOrdersSell { get; set; }
        public decimal? PickupOrdersSell { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? OnlineAmount { get; set; }
    }

    public class TotalItemValues
    {
        public int? TotalItemPosted { get; set; }
        public decimal? TotalItemValue { get; set; }
    }
}