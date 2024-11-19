using Application.Common;
using Application.Logging;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web.App_Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    [Authorize]
    public class ProductEntryController : Controller
    {
        private IProductService productService;
        private IProductImageService productImageService;
        private IActionLogService actionLogService;
        public ProductEntryController(IProductService productService, IProductImageService productImageService, IActionLogService actionLogService)
        {
            this.productService = productService;
            this.productImageService = productImageService;
            this.actionLogService = actionLogService;
        }
        
        public ActionResult Post()
        {
            return View();
        }

        public ActionResult BulkPost()
        {
            return View();
        }

        public ActionResult EditPost()
        {
            return View();
        }

        public ActionResult StockUpdate()
        {
            return View();
        }

        public ActionResult EditCategory()
        {
            return View();
        }

        public ActionResult PostProductMessage()
        {
            return View();
        }

        [HttpGet]
        public JsonResult IsBarcodeExists(string barcode)
        {
            bool isExists = productService.IsBarcodeExists(barcode);

            return Json(new
            {
                isExists = isExists,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGeneratedBarcode()
        {
            int code = GetProductCode();
            string productCode = code.ToString().PadLeft(5, '0');
            string clientCode = Utils.GetConfigValue("BarcodePrefix");

            string barcode = "0" + clientCode + productCode + "9";

            return Json(new
            {
                barcode
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostProduct()
        {
            string productJson = Request.Form[0];
            Product product = JsonConvert.DeserializeObject<Product>(productJson);            

            if (ModelState.IsValid)
            {
                bool isSuccess = true;
                string productId = Guid.NewGuid().ToString();
                bool isAdmin = User.IsInRole("admin") ? true : false;

                string copyImages = String.Empty;
                if (!String.IsNullOrEmpty(product.Status))
                {
                    copyImages = product.Status;
                }
                
                // Barcode can't be duplicate
                if (!String.IsNullOrEmpty(product.Barcode))
                {
                    bool isBarcodeExists = this.productService.IsBarcodeExists(product.BranchId, product.Barcode);                    
                    if (isBarcodeExists)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            message = "Product barcode is already exists!"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        int productCode = GetProductCode();
                        product.Id = productId;
                        product.UserId = Utils.GetLoggedInUser().Id;
                        product.LowStockAlert = 5;
                        product.IsApproved = true;
                        product.Status = EAdStatus.Running.ToString();
                        product.IsSync = false;
                        product.CostPrice = product.CostPrice == null ? 0 : product.CostPrice;
                        product.RetailPrice = product.RetailPrice == null ? 0 : product.RetailPrice;
                        product.WholesalePrice = product.WholesalePrice == null ? 0 : product.WholesalePrice;
                        product.OnlinePrice = product.OnlinePrice == null ? 0 : product.OnlinePrice;
                        product.ActionDate = DateTime.Now;
                                                
                        // Post user
                        this.productService.CreateProduct(product);

                        if (isSuccess)
                        {
                            // Save product images
                            isSuccess = AppUtils.SaveProductImage(productImageService, Request, productId, true);
                            
                            // Now complete the transaction
                            tran.Complete();
                        }
                    }
                    catch (Exception ex)
                    {                        
                        isSuccess = false;
                        ErrorLog.LogError(ex);
                    }
                }

                return Json(new
                {
                    isSuccess = isSuccess,                    
                }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateProduct(Product product)
        {
            bool isSuccess = true;

            // Barcode can't be duplicate
            if (!String.IsNullOrEmpty(product.Barcode))
            {
                bool isBarcodeExists = this.productService.IsBarcodeExists(product.Barcode, product.Id);
                if (isBarcodeExists)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "Product barcode is already exists!"
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            try
            {
                Product prodToUpdate = this.productService.GetProduct(product.Id);
                if (prodToUpdate != null)
                {
                    prodToUpdate.Title = product.Title;
                    prodToUpdate.Barcode = product.Barcode;
                    prodToUpdate.ShortCode = product.ShortCode;
                    prodToUpdate.BranchId = product.BranchId;
                    prodToUpdate.SupplierId = product.SupplierId;
                    prodToUpdate.ItemTypeId = product.ItemTypeId;
                    prodToUpdate.Description = product.Description;
                    prodToUpdate.CostPrice = product.CostPrice == null ? 0 : product.CostPrice;
                    
                    prodToUpdate.RetailPrice = product.RetailPrice == null ? 0 : product.RetailPrice;
                    prodToUpdate.WholesalePrice = product.WholesalePrice == null ? 0 : product.WholesalePrice;
                    prodToUpdate.OnlinePrice = product.OnlinePrice == null ? 0 : product.OnlinePrice;
                    
                    prodToUpdate.RetailDiscount = product.RetailDiscount;
                    prodToUpdate.WholesaleDiscount = product.WholesaleDiscount;
                    prodToUpdate.OnlineDiscount = product.OnlineDiscount;

                    prodToUpdate.Weight = product.Weight;
                    prodToUpdate.Unit = product.Unit;
                    prodToUpdate.Quantity = product.Quantity;
                    prodToUpdate.ExpireDate = product.ExpireDate;
                    prodToUpdate.LowStockAlert = product.LowStockAlert;

                    prodToUpdate.Color = product.Color;
                    prodToUpdate.Capacity = product.Capacity;
                    prodToUpdate.Condition = product.Condition;
                    prodToUpdate.IMEI = product.IMEI;
                    prodToUpdate.ModelNumber = product.ModelNumber;
                    prodToUpdate.Manufacturer = product.Manufacturer;
                    prodToUpdate.WarrantyPeriod = product.WarrantyPeriod;

                    prodToUpdate.IsFrozen = product.IsFrozen != null ? (bool)product.IsFrozen : false;
                    prodToUpdate.IsInternal = product.IsInternal != null ? (bool)product.IsInternal : false;
                    prodToUpdate.IsFeatured = product.IsFeatured != null ? (bool)product.IsFeatured : false;
                    prodToUpdate.IsFastMoving = product.IsFastMoving != null ? (bool)product.IsFastMoving : false;
                    prodToUpdate.IsMainItem = product.IsMainItem != null ? (bool)product.IsMainItem : false;
                    prodToUpdate.IsSync = false;
                    prodToUpdate.ActionDate = DateTime.Now;
                }

                this.productService.UpdateProduct(prodToUpdate);

                AppCommon.WriteActionLog(actionLogService, "Product", "Product Update", "Product Name: " + prodToUpdate.Title, "Update", User.Identity.Name);                
            }
            catch (Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError(ex);
            }

            return Json(new
            {
                isSuccess = isSuccess,
            }, JsonRequestBehavior.AllowGet);

        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateCategory(string productId, int categoryId)
        {
            bool isSuccess = true;
            try
            {
                Product prodToUpdate = this.productService.GetProduct(productId);
                if (prodToUpdate != null)
                {
                    prodToUpdate.CategoryId = categoryId;
                }

                this.productService.UpdateProduct(prodToUpdate);

                AppCommon.WriteActionLog(actionLogService, "Product", "Product Category Update", "Product Name: " + prodToUpdate.Title, "Update", User.Identity.Name);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError(ex);
            }

            return Json(new
            {
                isSuccess = isSuccess,
            }, JsonRequestBehavior.AllowGet);

        }

        public int GetProductCode()
        {
            int productCode = 0;
            try
            {
                string query = "SELECT IDENT_CURRENT('Products') as Code;";
                using (var context = new Data.Models.ApplicationEntities())
                {
                    var code = context.Database.SqlQuery<decimal>(query).FirstOrDefault();
                    if (code != 0)
                    {
                        productCode = Decimal.ToInt32(code) + 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return productCode;
        }
    }
}