using Application.Common;
using Application.Logging;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private IProductImageService productImageService;
        public PhotoController(IProductImageService productImageService)
        {
            this.productImageService = productImageService;
        }

        public ActionResult ManagePhoto()
        {
            return View();
        }

        public ActionResult SiteLogo()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SavePhoto(string productId)
        {
            bool isSuccess = false;
            string message = String.Empty;

            if (Request.Files == null || Request.Files[0] == null)
                return Json(new
                {
                    isSuccess = false,
                    message = "Please choose a user image!"
                }, JsonRequestBehavior.AllowGet);


            // Check max limit reached
            var photoList = this.productImageService.GetProductImages(productId, false);
            if (photoList.Count() >= 6)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "You image upload limit exceeded!"
                }, JsonRequestBehavior.AllowGet);
            }


            bool isPrimaryImage = photoList.Count() == 0 ? true : false;
            isSuccess = AppUtils.SaveProductImage(productImageService, Request, productId, false, isPrimaryImage);
            
            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }        

        public JsonResult SetPrimaryPhoto(string productId, string photoId)
        {            
            bool isSuccess = this.productImageService.SetProfileImage(productId, photoId);

            if (!isSuccess)
            {
                ErrorLog.LogError("Set Primary Image is Failed!");
            }

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPhotoList(string productId)
        {
            var imageList = this.productImageService.GetProductImages(productId, false);

            List<ProductImageViewModel> imageVMList = new List<ProductImageViewModel>();            
            
            if (imageList != null)
            {
                foreach (ProductImage pi in imageList)
                {
                    ProductImageViewModel pivm = new ProductImageViewModel();
                    pivm.Id = pi.Id;
                    pivm.ImageName = pi.ImageName;
                    pivm.ProductId = pi.ProductId;
                    pivm.DisplayOrder = pi.DisplayOrder == null ? 0 : (int)pi.DisplayOrder;
                    pivm.IsPrimaryImage = pi.IsPrimaryImage;
                    pivm.IsApproved = pi.IsApproved;
                    pivm.Status = pi.IsApproved ? "Approved" : "Pending";

                    imageVMList.Add(pivm);
                }
            }


            return Json(imageVMList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePhoto(string imageId)
        {
            bool isSuccess = true;

            isSuccess = AppUtils.DeleteProductImage(productImageService, imageId);
            
            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveLogo()
        {
            if (Request.Files == null || Request.Files[0] == null)
                return Json(new
                {
                    isSuccess = false,
                    message = "Please select a logo image!"
                }, JsonRequestBehavior.AllowGet);

            bool isSuccess = true;
            try
            {
                foreach (string name in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[name];

                    var fileName = "Logo.png";

                    var imagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Logo/Original/"), fileName);

                    // Save logo
                    file.SaveAs(imagePath);

                    // Save specified size
                    string imageSource = imagePath;
                    string imageDest = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Logo/"), fileName);
                    ImageResizer.Resize(imageSource, imageDest, 200, 60, false, ImageFormat.Jpeg);

                    isSuccess = true;
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError(ex, "Failed: Saving logo");
            }

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }        
    }
}