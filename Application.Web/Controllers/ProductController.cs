using Application.Common;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUserService userService;
        private IProductService productService;
        private IProductImageService productImageService;
        public ProductController(IUserService userService, IProductImageService productImageService, IProductService productService)
        {
            this.userService = userService;
            this.productService = productService;
            this.productImageService = productImageService;
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }

        public JsonResult GetProduct(string id)
        {
            ProductViewModel pvm = new ProductViewModel();

            Product product = productService.GetProduct(id);
            FillProduct(pvm, product);

            return Json(pvm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductByBarcode(string barcode)
        {
            ProductViewModel pvm = new ProductViewModel();

            Product product = productService.GetProductByBarcode(barcode);
            FillProduct(pvm, product);

            return Json(pvm, JsonRequestBehavior.AllowGet);
        }

        public void FillProduct(ProductViewModel pvm, Product product)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix();
            if (product != null)
            {
                pvm.Id = product.Id;
                pvm.Code = product.Code;
                pvm.Title = product.Title;
                pvm.Barcode = product.Barcode;
                pvm.ShortCode = product.ShortCode;
                pvm.BranchId = product.BranchId;
                pvm.SupplierId = product.SupplierId;
                pvm.ExpireDate = product.ExpireDate;
                pvm.TitleSEO = Utils.GenerateSeoTitle(product.Id, product.Title);
                pvm.Description = product.Description;
                pvm.CostPrice = product.CostPrice;

                pvm.RetailPrice = product.RetailPrice;
                pvm.WholesalePrice = product.WholesalePrice;
                pvm.OnlinePrice = product.OnlinePrice;

                pvm.RetailDiscount = product.RetailDiscount == null ? 0 : product.RetailDiscount;
                pvm.WholesaleDiscount = product.WholesaleDiscount == null ? 0 : product.WholesaleDiscount;
                pvm.OnlineDiscount = product.OnlineDiscount == null ? 0 : product.OnlineDiscount;

                pvm.Weight = product.Weight;
                pvm.Unit = product.Unit;
                pvm.Quantity = product.Quantity;
                pvm.ItemTypeId = product.ItemTypeId;
                pvm.LowStockAlert = product.LowStockAlert.Value ;
                pvm.Color = product.Color;
                pvm.Capacity = product.Capacity;
                pvm.Condition = product.Condition;
                pvm.Manufacturer = product.Manufacturer;
                pvm.IMEI = product.IMEI;
                pvm.ModelNumber = product.ModelNumber;
                pvm.WarrantyPeriod = product.WarrantyPeriod;
                pvm.IsFrozen = product.IsFrozen != null ? product.IsFrozen : false;
                pvm.IsInternal = product.IsInternal != null ? product.IsInternal : false;
                pvm.IsFeatured = product.IsFeatured != null ? product.IsFeatured : false;
                pvm.IsFastMoving = product.IsFastMoving != null ? product.IsFastMoving : false;
                pvm.IsMainItem = product.IsMainItem != null ? product.IsMainItem : false;
                pvm.WeightText = product.Weight != null ? (Convert.ToString((double)product.Weight) + " " + product.Unit) : "";
                pvm.PostingTime = "Posted: " + string.Format("{0:d MMMM yyyy hh:mm tt}", product.ActionDate);
                pvm.Gst = product.Gst;
                // Price & Discount
                string priceText = string.Empty;
                string oldPriceText = string.Empty;
                Utils.GetPrice(product.OnlinePrice, product.OnlineDiscount, out decimal newPrice, out priceText, out oldPriceText);
                pvm.OnlinePrice = newPrice;
                pvm.PriceText = priceText;
                pvm.PriceTextOld = oldPriceText;
                pvm.Options = product.Option != null ? product.Option.Split(',') : new string[0];
                // Product images
                pvm.ImageList = new List<ProductImageViewModel>();
                IEnumerable<ProductImage> imageList = productImageService.GetProductImages(product.Id);
                if (imageList != null)
                {
                    if (imageList.Count() > 0)
                    {
                        foreach (ProductImage pi in imageList)
                        {
                            ProductImageViewModel pivm = new ProductImageViewModel
                            {
                                ImageName = imageSrcPrefix + "/Large/" + pi.ImageName,
                                ThumbImageName = imageSrcPrefix + "/Small/" + pi.ImageName,
                                MaxViewImageName = imageSrcPrefix + "/XLarge/" + pi.ImageName,
                                Id = pi.Id,
                                ProductId = pi.ProductId,
                                DisplayOrder = pi.DisplayOrder == null ? 0 : (int)pi.DisplayOrder,
                                IsPrimaryImage = pi.IsPrimaryImage,
                                IsApproved = pi.IsApproved
                            };
                            pvm.ImageList.Add(pivm);
                        }
                    }
                    else
                    {
                        ProductImageViewModel pivm = new ProductImageViewModel
                        {
                            ImageName = imageSrcPrefix + "/Large/no-image.jpg"
                        };
                        pvm.ImageList.Add(pivm);
                    }
                }

                // Product category
                pvm.Category = new CategoryViewModel
                {
                    Id = product.CategoryId
                };

                // Breadcrumb
                List<BreadCrumbViewModel> bcList = new List<BreadCrumbViewModel>();
                BreadCrumbViewModel bc = new BreadCrumbViewModel();
                if (product.Category != null)
                {
                    bc = new BreadCrumbViewModel
                    {
                        Id = product.Category.Id.ToString(),
                        Name = product.Category.Name
                    };
                    bcList.Add(bc);

                    if (product.Category.Category2 != null)
                    {
                        bc = new BreadCrumbViewModel
                        {
                            Id = product.Category.Category2.Id.ToString(),
                            Name = product.Category.Category2.Name
                        };
                        bcList.Add(bc);

                        if (product.Category.Category2.Category2 != null)
                        {
                            bc = new BreadCrumbViewModel
                            {
                                Id = product.Category.Category2.Category2.Id.ToString(),
                                Name = product.Category.Category2.Category2.Name
                            };
                            bcList.Add(bc);

                            if (product.Category.Category2.Category2.Category2 != null)
                            {
                                bc = new BreadCrumbViewModel
                                {
                                    Id = product.Category.Category2.Category2.Category2.Id.ToString(),
                                    Name = product.Category.Category2.Category2.Category2.Name
                                };
                                bcList.Add(bc);
                            }
                        }
                    }
                }

                bc = new BreadCrumbViewModel
                {
                    Id = "",
                    Name = product.Title.Substring(0, product.Title.Length <= 60 ? product.Title.Length : 60).Trim()
                };
                bcList.Add(bc);

                pvm.BreadCrumbList = bcList;

                pvm.Seller = new UserViewModel
                {
                    Username = product.User.Username,
                    Id = product.User.Id,
                    CreatDate = product.User.CreateDate,
                    MemberSince = Utils.GetDateDifference(DateTime.Now, product.User.CreateDate)
                };

                // Update view count
                productService.UpdateViewCount(product.Id);

                string sqlColor = $"Select Id, ProductId, Color from ProductColor where ProductId = '{product.Id}'";
                string sqlSize = $"Select Id, ProductId, Size from ProductSize where ProductId = '{product.Id}'";
                string sqlSizeColor = $"Select Id, ProductId, Size, Color, Price from ProductSizeColor where ProductId = '{product.Id}'";
                string sqlProductChoice = $"select c.Id, c.Name, c.Amount from ProductChoice pc inner join Choice c on pc.ChoiceId = c.Id where pc.ProductId = '{product.Id}'";

                List<ColorViewModel> colors = new List<ColorViewModel>();
                List<SizeViewModel> sizes = new List<SizeViewModel>();
                List<SizeColorViewModel> sizeColors = new List<SizeColorViewModel>();
                List<ProductChoiceViewModel> productChoices = new List<ProductChoiceViewModel>();

                using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
                {
                    colors = context.Database.SqlQuery<ColorViewModel>(sqlColor).ToList();
                    sizes = context.Database.SqlQuery<SizeViewModel>(sqlSize).ToList();
                    sizeColors = context.Database.SqlQuery<SizeColorViewModel>(sqlSizeColor).ToList();
                    productChoices = context.Database.SqlQuery<ProductChoiceViewModel>(sqlProductChoice).ToList();
                }

                pvm.Colors = colors;
                pvm.Sizes = sizes;
                pvm.SizeColors = sizeColors;
                pvm.Choices = productChoices;
            }
        }

        public JsonResult GetRelatedProducts(int categoryId, string excludeProductId)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix() + "/Small/";
            List<ProductViewModel> rpVMList = new List<ProductViewModel>();
            List<Product> recordList = productService.GetRelatedProducts(13, categoryId).ToList();

            if (recordList != null && recordList.Count > 0)
            {
                foreach (Product p in recordList)
                {
                    ProductViewModel rpVM = new ProductViewModel();
                    if (p.Id == excludeProductId)
                    {
                        continue;
                    }

                    rpVM.Id = p.Id;
                    rpVM.Title = p.Title;

                    // Price & Discount
                    string priceText = string.Empty;
                    string oldPriceText = string.Empty;
                    Utils.GetPrice(p.OnlinePrice, p.OnlineDiscount, out decimal newPrice, out priceText, out oldPriceText);
                    rpVM.OnlinePrice = newPrice;
                    rpVM.PriceText = priceText;
                    rpVM.PriceTextOld = oldPriceText;

                    // Get user images
                    List<ProductImageViewModel> imageVMList = new List<ProductImageViewModel>();
                    IEnumerable<ProductImage> imageList = productImageService.GetProductImages(p.Id, true);
                    foreach (ProductImage image in imageList)
                    {
                        if (image.IsPrimaryImage)
                        {
                            rpVM.PrimaryImageName = imageSrcPrefix + image.ImageName;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(rpVM.PrimaryImageName))
                    {
                        rpVM.PrimaryImageName = imageSrcPrefix + "no-image.jpg";
                    }

                    rpVMList.Add(rpVM);
                }

                if (rpVMList.Count() == 13)
                {
                    rpVMList.Remove(rpVMList.Last());
                }
            }

            return Json(rpVMList, JsonRequestBehavior.AllowGet);
        }

        public string GetAllChildIds(string id, bool isCategory)
        {
            string sqlQuery = string.Empty;
            string allChildIds = id;

            if (isCategory)
            {
                sqlQuery = string.Format(@"
                                            ;WITH r as (
                                             SELECT ID
                                             FROM Category
                                             WHERE ParentID = {0}
                                             UNION ALL
                                             SELECT d.ID 
                                             FROM Category d
                                                INNER JOIN r 
                                                   ON d.ParentID = r.ID
                                        )
                                        SELECT ID FROM r ", id);
            }

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<ChildIds> recordList = context.Database.SqlQuery<ChildIds>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (ChildIds record in recordList)
                    {
                        allChildIds += "," + record.Id;
                    }
                }
            }

            return allChildIds;
        }

        [HttpPost]
        public JsonResult SearchResult(SearchModel searchModel)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix() + "/Grid/";

            if (!string.IsNullOrEmpty(searchModel.CategoryId))
            {
                string[] catIds = searchModel.CategoryId.Split(',');
                if (catIds.Count() == 1)
                {
                    searchModel.CategoryId = GetAllChildIds(searchModel.CategoryId, true);
                }
            }

            string loggedInUserId = (Utils.GetLoggedInUser()) != null ? Utils.GetLoggedInUser().Id : string.Empty;
            int pageNumber = searchModel.PageNo;
            int pageSize = 28;

            int recordFrom = (pageNumber - 1) * pageSize + 1;
            int recordTo = recordFrom + (pageSize - 1);

            // Sort order
            string orderBy = " p.ActionDate DESC";
            if (!string.IsNullOrEmpty(searchModel.SortOrder))
            {
                switch (searchModel.SortOrder)
                {
                    case "DatePosted":
                        orderBy = " p.ActionDate DESC";
                        break;
                    case "PriceLow":
                        orderBy = " p.Price ASC";
                        break;
                    case "PriceHigh":
                        orderBy = " p.Price DESC";
                        break;
                    default:
                        orderBy = " p.ActionDate DESC";
                        break;
                }
            }

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
            string whereQuery = string.Format(@" WHERE IsApproved = 1 AND IsDeleted = 0 ");

            if (!string.IsNullOrEmpty(searchModel.OnlyDiscount))
            {
                whereQuery += " AND OnlineDiscount > 0 ";
            }

            if (!string.IsNullOrEmpty(searchModel.CategoryId))
            {
                whereQuery += " AND CategoryId IN (" + searchModel.CategoryId.ToString() + ")";
            }

            if (!string.IsNullOrEmpty(searchModel.LocationId))
            {
                whereQuery += " AND LocationId IN (" + searchModel.LocationId.ToString() + ")";
            }

            // Price
            if (searchModel.MinPrice > 0)
            {
                whereQuery += " AND Price >= " + searchModel.MinPrice.ToString() + "";
            }
            if (searchModel.MaxPrice > 0)
            {
                whereQuery += " AND Price <= " + searchModel.MaxPrice.ToString() + "";
            }

            // Free text search (adding OR clause)
            if (!string.IsNullOrEmpty(searchModel.FreeText))
            {
                string freeTextWhereClause = string.Empty;
                string[] searchTextList = searchModel.FreeText.Split();
                foreach (string searchText in searchTextList)
                {
                    if (!string.IsNullOrEmpty(freeTextWhereClause))
                    {
                        freeTextWhereClause += " OR ";
                    }

                    freeTextWhereClause += " Title LIKE '%" + searchText + "%' OR Description LIKE '%" + searchText + "%' ";
                }

                if (!string.IsNullOrEmpty(freeTextWhereClause))
                {
                    freeTextWhereClause = " AND (" + freeTextWhereClause + ")";
                    whereQuery += freeTextWhereClause;
                }
            }

            // Paging clause
            string pagingQuery = string.Format(@" WHERE RowNum BETWEEN {0} AND {1}", recordFrom, recordTo);

            string sqlQuery = selectRecords.Replace("#WHERE#", whereQuery) + pagingQuery;
            string sqlTotalRecords = selectTotalCount + whereQuery;

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<ProductViewModel> recordList = context.Database.SqlQuery<ProductViewModel>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    List<ProductViewModel> newRecordList = new List<ProductViewModel>();
                    foreach (ProductViewModel p in recordList)
                    {
                        // Product Price
                        if (p.OnlinePrice != null)
                        {
                            // Price & Discount
                            string priceText = string.Empty;
                            string oldPriceText = string.Empty;
                            Utils.GetPrice(p.OnlinePrice, p.OnlineDiscount, out decimal newPrice, out priceText, out oldPriceText);
                            p.OnlinePrice = newPrice;
                            p.PriceText = priceText;
                            p.PriceTextOld = oldPriceText;
                        }

                        // Get user images
                        List<ProductImageViewModel> imageVMList = new List<ProductImageViewModel>();
                        IEnumerable<ProductImage> imageList = productImageService.GetProductImages(p.Id, true);
                        foreach (ProductImage image in imageList)
                        {
                            ProductImageViewModel imageVM = new ProductImageViewModel
                            {
                                ProductId = p.Id,
                                ImageName = image.ImageName,
                                DisplayOrder = image.DisplayOrder != null ? (int)image.DisplayOrder : 0,
                                IsApproved = image.IsApproved,
                                IsPrimaryImage = image.IsPrimaryImage
                            };
                            if (image.IsPrimaryImage)
                            {
                                p.PrimaryImageName = imageSrcPrefix + image.ImageName;
                            }

                            imageVMList.Add(imageVM);
                        }

                        if (string.IsNullOrEmpty(p.PrimaryImageName))
                        {
                            p.PrimaryImageName = imageSrcPrefix + "no-image.jpg";
                        }

                        p.ImageList = imageVMList;

                        // Post time
                        string postingTime = "Posted: ";
                        TimeSpan span = (DateTime.Now - p.ActionDate);
                        int min = span.Minutes;
                        int hour = span.Hours;
                        int day = span.Days;

                        if (day > 0)
                        {
                            postingTime += day > 1 ? day.ToString() + " days" : day.ToString() + " day";
                        }
                        else if (hour > 0)
                        {
                            postingTime += hour > 1 ? hour.ToString() + " hours" : hour.ToString() + " hour";
                        }
                        else if (min > 0)
                        {
                            postingTime += min > 1 ? min.ToString() + " minutes" : min.ToString() + " minute";
                        }
                        else
                        {
                            postingTime += " few seconds";
                        }
                        postingTime += " ago";
                        p.PostingTime = postingTime;
                    }
                }

                // Get total records
                int totalPages = 0;
                int totalRecords = 0;
                if (searchModel.IsGetTotalRecord)
                {
                    TotalRecordsViewModel item = context.Database.SqlQuery<TotalRecordsViewModel>(sqlTotalRecords).FirstOrDefault();
                    totalPages = (int)Math.Ceiling((double)item.TotalRecords / pageSize);
                    totalRecords = item.TotalRecords;
                }

                return Json(new
                {
                    recordList = recordList,
                    totalPages = totalPages,
                    totalRecords = totalRecords
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetCouponDiscount(string coupon)
        {

            string selectRecords = string.Format(@"
                                            SELECT * FROM Promotions 
                                                #WHERE# ");
            string whereQuery = string.Format($" WHERE Coupon = '{coupon}' And '{DateTime.Now.Date.ToString("yyyy-MM-dd")}' between StartDate AND EndDate ");

            string sqlQuery = selectRecords.Replace("#WHERE#", whereQuery);
            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                PromotionViewModel record = context.Database.SqlQuery<PromotionViewModel>(sqlQuery).FirstOrDefault();
                if (record != null)
                {
                    return Json(new
                    {
                        IsSuccess = true,
                        Value = record.Percentage
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new
            {
                IsSuccess = false,
                value = 0
            }, JsonRequestBehavior.AllowGet);
        }
    }

    public class TotalRecordsViewModel
    {
        public int TotalRecords { get; set; }
    }

    public class ChildIds
    {
        public int Id { get; set; }
    }
}