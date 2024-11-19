using Application.Common;
using Application.Logging;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Application.Web
{
    public static class AppUtils
    {

        public static List<ColorViewModel> Colors { get; set; }
        public static List<SizeViewModel> Sizes { get; set; }
        public static List<ProductChoiceViewModel> Choices { get; set; }

        public static UserViewModel GetLoggedInUserInfo(User user)
        {
            UserViewModel userVM = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                IsActive = user.IsActive,
                IsDelete = user.IsDelete,
                IsVerified = user.IsVerified
            };

            return userVM;
        }

        public static bool SaveProductImage(IProductImageService service, HttpRequestBase request, string productId, bool isFromPosting, bool isPrimaryImage = false)
        {
            bool isSuccess = true;
            try
            {
                int count = 1;
                foreach (string name in request.Files)
                {
                    HttpPostedFileBase file = request.Files[name];

                    string originalFileName = file.FileName;
                    string fileExtension = Path.GetExtension(originalFileName);
                    string productImageId = Guid.NewGuid().ToString();

                    string fileName = productImageId + ".jpg";
                    string imagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Original/"), fileName);

                    // Save photo
                    file.SaveAs(imagePath);

                    // Saving photo in different sizes
                    string imageSource = string.Empty;
                    string imageDest = string.Empty;

                    // Small
                    imageSource = imagePath;
                    imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Small/"), fileName);
                    ImageResizer.Resize_AspectRatio(imageDest, imageSource, 140, 140);

                    // Medium
                    imageSource = imagePath;
                    imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Medium/"), fileName);
                    ImageResizer.Resize_AspectRatio(imageDest, imageSource, 200, 200);

                    // Large
                    imageSource = imagePath;
                    imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Large/"), fileName);
                    ImageResizer.Resize_AspectRatio(imageDest, imageSource, 650, 650);

                    // XLarge
                    imageSource = imagePath;
                    imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/XLarge/"), fileName);
                    ImageResizer.Resize_AspectRatio(imageDest, imageSource, 800, 800);

                    // Home page grid
                    imageSource = imagePath;
                    imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Grid/"), fileName);
                    ImageResizer.Resize_AspectRatio(imageDest, imageSource, 250, 250);

                    // Save records to db
                    ProductImage productImage = new ProductImage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = productId,
                        ImageName = fileName,
                        DisplayOrder = count,
                        IsApproved = true,
                        ActionDate = DateTime.Now
                    };

                    if (isFromPosting)
                    {
                        productImage.IsPrimaryImage = count == 1 ? true : false;
                    }
                    else
                    {
                        productImage.IsPrimaryImage = isPrimaryImage;
                    }

                    service.CreateProductImage(productImage);

                    isSuccess = true;
                    count++;
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError(ex, "Failed: Saving user image");
            }

            return isSuccess;
        }


        public static bool DeleteProductImage(IProductImageService productImageService, string imageId)
        {
            bool isSuccess = true;
            ProductImage productImage = productImageService.GetProductImage(imageId);
            if (productImage != null)
            {
                // Delete from db
                isSuccess = productImageService.DeleteImage(imageId);

                if (isSuccess)
                {
                    // Delete from file
                    string file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/XLarge/"), productImage.ImageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Large/"), productImage.ImageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Medium/"), productImage.ImageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Original/"), productImage.ImageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Small/"), productImage.ImageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Grid/"), productImage.ImageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }
                }
                else
                {
                    isSuccess = false;
                    ErrorLog.LogError("Delete Image Failed");
                }
            }

            return isSuccess;
        }

        public static List<HomePageItem> GetHomePageItemsData_ByCategory(string categoryIds)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix();

            string sqlQuery = string.Format(@"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                            from Products p, ProductImages pi
                            where p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                            and CategoryId In ({0})
                            order by ViewCount desc", categoryIds);

            List<HomePageItem> itemList = new List<HomePageItem>();

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<HomePageItem> recordList = context.Database.SqlQuery<HomePageItem>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (HomePageItem product in recordList)
                    {
                        // Price & Discount
                        string priceText = string.Empty;
                        string oldPriceText = string.Empty;
                        decimal newPrice = 0;
                        Utils.GetPrice(product.OnlinePrice, product.Discount, out newPrice, out priceText, out oldPriceText);
                        product.OnlinePrice = newPrice;
                        product.PriceText = priceText;
                        product.PriceTextOld = oldPriceText;

                        product.PrimaryImageName = imageSrcPrefix + "/Grid/" + product.PrimaryImageName;
                    }

                    itemList = recordList;
                }
            }

            return itemList;
        }

        public static List<HomePageItem> GetHomePage_Products(int categoryId = 0)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix();
            string sqlQuery = string.Empty;

            if (categoryId == 0)
            {
                sqlQuery = @"select 24 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName,Gst
                            from Products p left join ProductImages pi on  p.Id = pi.ProductId
                            where p.IsApproved = 1 and p.IsDeleted = 0
                            and p.IsFeatured = 1";
            }
            else
            {
                sqlQuery = @"select p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, Gst
                            from Products p, ProductImages pi
                            where p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                            and p.CategoryId = '" + categoryId + "' ";
            }

            List<HomePageItem> itemList = new List<HomePageItem>();

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<HomePageItem> recordList = context.Database.SqlQuery<HomePageItem>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (HomePageItem product in recordList)
                    {
                        // Price & Discount
                        string priceText = string.Empty;
                        string oldPriceText = string.Empty;
                        decimal newPrice = 0;
                        Utils.GetPrice(product.OnlinePrice, product.Discount, out newPrice, out priceText, out oldPriceText);
                        product.OnlinePrice = newPrice;
                        product.PriceText = priceText;
                        product.PriceTextOld = oldPriceText;

                        product.PrimaryImageName = imageSrcPrefix + "/Grid/" + product.PrimaryImageName;

                    }

                    itemList = recordList;
                    IsSizeColorDataAvailable(itemList, context);


                }
            }

            return itemList;
        }
        public static List<HomePageItem> GetHomePage_FeaturedItems(string productId = null)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix();
            string sqlQuery = string.Empty;

            if (string.IsNullOrEmpty(productId))
            {
                //sqlQuery = @"select top 24 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName,Gst
                //            from Products p, ProductImages pi
                //            where p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                //            and p.IsFeatured = 1";
                sqlQuery = @"select top 24 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName,Gst
                            from Products p left join ProductImages pi on  p.Id = pi.ProductId
                            where p.IsApproved = 1 and p.IsDeleted = 0
                            and p.IsFeatured = 1";
            }
            else
            {
                sqlQuery = @"select top 24 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, Gst
                            from Products p, ProductImages pi
                            where p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                            and p.Id = '" + productId + "' ";
            }

            List<HomePageItem> itemList = new List<HomePageItem>();

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<HomePageItem> recordList = context.Database.SqlQuery<HomePageItem>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (HomePageItem product in recordList)
                    {
                        // Price & Discount
                        string priceText = string.Empty;
                        string oldPriceText = string.Empty;
                        decimal newPrice = 0;
                        Utils.GetPrice(product.OnlinePrice, product.Discount, out newPrice, out priceText, out oldPriceText);
                        product.OnlinePrice = newPrice;
                        product.PriceText = priceText;
                        product.PriceTextOld = oldPriceText;

                        product.PrimaryImageName = imageSrcPrefix + "/Grid/" + product.PrimaryImageName;

                    }

                    itemList = recordList;
                    IsSizeColorDataAvailable(itemList, context);


                }
            }

            return itemList;
        }


        public static List<CustomerViewModel> GetHomePage_Customers()
        {
            string sqlQuery = string.Empty;

            sqlQuery = @"select id, code, CustName,CustEmail,CustAddress,CustPhone,Suburb,
                            PostCode, LogBy, Logtime, LastUpdate from Customers";

            List<CustomerViewModel> itemList = new List<CustomerViewModel>();

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                itemList = context.Database.SqlQuery<CustomerViewModel>(sqlQuery).ToList();
            }

            return itemList;
        }

        public static void GetGlobalData()
        {
            string sqlColor = "Select Id, ProductId, Color from ProductColor";
            string sqlSize = "Select Id, ProductId, Size from ProductSize";
            string sqlProductChoice = "select Id,ProductId from ProductChoice";

            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                Colors = context.Database.SqlQuery<ColorViewModel>(sqlColor).ToList();
                Sizes = context.Database.SqlQuery<SizeViewModel>(sqlSize).ToList();
                Choices = context.Database.SqlQuery<ProductChoiceViewModel>(sqlProductChoice).ToList();
            }
        }

        private static void IsSizeColorDataAvailable(List<HomePageItem> itemList, Data.Models.ApplicationEntities context)
        {
            foreach (HomePageItem item in itemList)
            {
                //string sqlColor = "Select Count(*) from ProductColor where ProductId =  '" + item.Id + "'";
                //string sqlSize = "Select Count(*) from ProductSize where ProductId = '" + item.Id + "'";
                //int colorCount = context.Database.SqlQuery<int>(sqlColor).FirstOrDefault();
                //int sizeCount = context.Database.SqlQuery<int>(sqlSize).FirstOrDefault();

                int colorCount = Colors.Where(x => x.ProductId == item.Id).Count();
                int sizeCount = Sizes.Where(x => x.ProductId == item.Id).Count();
                int choices = Choices.Where(x => x.ProductId == item.Id).Count();
                if (colorCount != 0 || sizeCount != 0 || choices != 0)
                {
                    item.IsSizeColorAvailable = true;
                }
            }
        }
        public static List<HomePageItem> GetHomePage_PopularItems(string productId = null)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix();
            string sqlQuery = string.Empty;

            if (string.IsNullOrEmpty(productId))
            {
                //sqlQuery = @"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                //            from Products p, ProductImages pi
                //            where p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                //            order by SoldCount desc";
                sqlQuery = @"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                            from Products p left join ProductImages pi on p.Id = pi.ProductId
                            where p.IsApproved = 1 and p.IsDeleted = 0
                            order by SoldCount desc";
            }
            else
            {
                sqlQuery = @"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                            from Products p, ProductImages pi
                            where p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                            and p.Id = '" + productId + "' ";
            }

            List<HomePageItem> itemList = new List<HomePageItem>();

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<HomePageItem> recordList = context.Database.SqlQuery<HomePageItem>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (HomePageItem product in recordList)
                    {
                        // Price & Discount
                        string priceText = string.Empty;
                        string oldPriceText = string.Empty;
                        decimal newPrice = 0;
                        Utils.GetPrice(product.OnlinePrice, product.Discount, out newPrice, out priceText, out oldPriceText);
                        product.OnlinePrice = newPrice;
                        product.PriceText = priceText;
                        product.PriceTextOld = oldPriceText;

                        product.PrimaryImageName = imageSrcPrefix + "/Grid/" + product.PrimaryImageName;
                    }

                    itemList = recordList;
                    IsSizeColorDataAvailable(itemList, context);
                }
            }

            return itemList;
        }

        public static List<HomePageItem> GetHomePage_NewArrivals(string productId = null)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix();
            string sqlQuery = string.Empty;

            if (string.IsNullOrEmpty(productId))
            {
                //sqlQuery = @"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                //            from Products p, ProductImages pi
                //            where p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                //            order by p.ActionDate desc";
                sqlQuery = @"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                            from Products p left join  ProductImages pi on p.Id = pi.ProductId
                            where p.IsApproved = 1 and p.IsDeleted = 0
                            order by p.ActionDate desc";
            }
            else
            {
                sqlQuery = @"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                            from Products p, ProductImages pi
                            where p.Id = ps.Id       
                            and p.Id = pi.ProductId and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
                            and p.Id = '" + productId + "' ";
            }

            List<HomePageItem> itemList = new List<HomePageItem>();

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                // Get search records
                List<HomePageItem> recordList = context.Database.SqlQuery<HomePageItem>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (HomePageItem product in recordList)
                    {
                        // Price & Discount
                        string priceText = string.Empty;
                        string oldPriceText = string.Empty;
                        decimal newPrice = 0;
                        Utils.GetPrice(product.OnlinePrice, product.Discount, out newPrice, out priceText, out oldPriceText);
                        product.OnlinePrice = newPrice;
                        product.PriceText = priceText;
                        product.PriceTextOld = oldPriceText;

                        product.PrimaryImageName = imageSrcPrefix + "/Grid/" + product.PrimaryImageName;

                    }

                    itemList = recordList;
                    IsSizeColorDataAvailable(itemList, context);
                }
            }

            return itemList;
        }



        //------------------------------------------
        public static List<HomePageItem> GetHomepageCategoryItems(int categoryId)
        {
            string imageSrcPrefix = Utils.GetProductImageSrcPrefix();
            string sqlQuery = string.Empty;

            string catIds = GetAllChildIds(categoryId.ToString());

            sqlQuery = string.Format(@"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName, p.Gst
                            from Products p, ProductImages pi, Category c
                            where p.Id = pi.ProductId and p.CategoryId = c.Id and pi.IsPrimaryImage = 1 and p.IsDeleted = 0
                            and c.Id in ({0})
                            order by ViewCount desc", catIds);

            List<HomePageItem> itemList = new List<HomePageItem>();

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<HomePageItem> recordList = context.Database.SqlQuery<HomePageItem>(sqlQuery).ToList();
                if (recordList != null && recordList.Count > 0)
                {
                    foreach (HomePageItem product in recordList)
                    {
                        // Price & Discount
                        string priceText = string.Empty;
                        string oldPriceText = string.Empty;
                        decimal newPrice = 0;
                        Utils.GetPrice(product.OnlinePrice, product.Discount, out newPrice, out priceText, out oldPriceText);
                        product.OnlinePrice = newPrice;
                        product.PriceText = priceText;
                        product.PriceTextOld = oldPriceText;

                        product.PrimaryImageName = imageSrcPrefix + "/Grid/" + product.PrimaryImageName;

                    }

                    itemList = recordList;
                }
            }

            return itemList;
        }
        //-*****************************************



        //public static List<HomePageItem> GetHomePageItemsData_CategoryItems(int categoryId)
        //{
        //    var imageSrcPrefix = Utils.GetProductImageSrcPrefix();
        //    string sqlQuery = String.Empty;

        //    string catIds = GetAllChildIds(categoryId.ToString());

        //    sqlQuery = String.Format(@"select top 10 p.Id as Id, p.Title as Title, p.OnlinePrice as OnlinePrice, p.OnlineDiscount as Discount, pi.ImageName as PrimaryImageName
        //                    from Products p, ProductImages pi, Category c
        //                    where p.Id = pi.ProductId and p.CategoryId = c.Id and pi.IsPrimaryImage = 1 and pi.IsApproved = 1 and p.IsApproved = 1 and p.IsDeleted = 0
        //                    and c.Id in ({0})
        //                    order by SoldCount desc", catIds);

        //    List<HomePageItem> itemList = new List<HomePageItem>();

        //    Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
        //    using (var context = new Data.Models.ApplicationEntities())
        //    {
        //        var recordList = context.Database.SqlQuery<HomePageItem>(sqlQuery).ToList();
        //        if (recordList != null && recordList.Count > 0)
        //        {
        //            foreach (var product in recordList)
        //            {
        //                // Price & Discount
        //                string priceText = String.Empty;
        //                string oldPriceText = String.Empty;
        //                decimal newPrice = 0;
        //                Utils.GetPrice(product.OnlinePrice, product.Discount, out newPrice, out priceText, out oldPriceText);
        //                product.OnlinePrice = newPrice;
        //                product.PriceText = priceText;
        //                product.PriceTextOld = oldPriceText;

        //                product.PrimaryImageName = imageSrcPrefix + "/Grid/" + product.PrimaryImageName;

        //            }

        //            itemList = recordList;
        //        }
        //    }

        //    return itemList;
        //}

        public static string GetAllChildIds(string id)
        {
            string sqlQuery = string.Empty;
            string allChildIds = id;

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

            Application.Data.Models.ApplicationEntities db = new Data.Models.ApplicationEntities();
            using (Data.Models.ApplicationEntities context = new Data.Models.ApplicationEntities())
            {
                List<ChildIdsViewModel> recordList = context.Database.SqlQuery<ChildIdsViewModel>(sqlQuery).ToList();

                if (recordList != null && recordList.Count > 0)
                {
                    foreach (ChildIdsViewModel record in recordList)
                    {
                        allChildIds += "," + record.Id;
                    }
                }
            }

            return allChildIds;
        }

        private class ChildIdsViewModel
        {
            public int Id { get; set; }
        }
    }



}