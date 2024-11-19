using Application.Common;
using Application.Logging;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    [Authorize]
    public class HomeSliderController : Controller
    {
        private IProductImageService productImageService;
        private ISliderImageService sliderImageService;

        public HomeSliderController(IProductImageService productImageService, ISliderImageService sliderImageService)
        {
            this.productImageService = productImageService;
            this.sliderImageService = sliderImageService;
        }

        public ActionResult SliderImage()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetSliderImageList()
        {
            List<SliderImage> list = new List<SliderImage>();

            ObjectCache cache = MemoryCache.Default;
            if (cache.Contains(ConstKey.ckHomeSlider))
            {
                list = (List<SliderImage>)cache.Get(ConstKey.ckHomeSlider);
            }
            else
            {
                list = this.sliderImageService.GetSliderImages().ToList();
                
                // Store data in the cache
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.SlidingExpiration = TimeSpan.FromDays(1);
                cache.Add(ConstKey.ckHomeSlider, list, cacheItemPolicy);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSliderImage(string url, int displayOrder)
        {
            if (Request.Files == null || Request.Files[0] == null)
                return Json(new
                {
                    isSuccess = false,
                    message = "Please choose a slider image!"
                }, JsonRequestBehavior.AllowGet);

            bool isSuccess = true;
            try
            {
                foreach (string name in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[name];

                    var originalFileName = file.FileName;
                    var fileExtension = Path.GetExtension(originalFileName);                    
                    var fileName = Guid.NewGuid().ToString() + ".jpg";

                    var imagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Slider/Original/"), fileName);

                    // Save photo
                    file.SaveAs(imagePath);

                    // Save specified size
                    string imageSource = imagePath;
                    string imageDest = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Slider/"), fileName);
                    //ImageResizer.Resize(imageSource, imageDest, 1076, 350, false, ImageFormat.Jpeg);
                    ImageResizer.Resize(imageSource, imageDest, 1900, 400, false, ImageFormat.Jpeg);

                    // Save records to db
                    SliderImage si = new Model.Models.SliderImage();
                    si.ImageName = fileName;
                    si.Url = url;
                    si.DisplayOrder = displayOrder;
                    this.sliderImageService.CreateSliderImage(si);

                    // Remove slider cache
                    ObjectCache cache = MemoryCache.Default;
                    cache.Remove(ConstKey.ckHomeSlider);

                    isSuccess = true;                    
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                ErrorLog.LogError(ex, "Failed: Saving user image");
            }

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSliderImage(string imageName)
        {
            bool isSuccess = true;
            SliderImage sliderImage = this.sliderImageService.GetSliderImage(imageName);
            if (sliderImage != null)
            {
                // Delete from db
                isSuccess = this.sliderImageService.DeleteSliderImage(imageName);

                // Delete from file
                if (isSuccess)
                {
                    string file = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Slider/Original/"), imageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    file = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Images/Slider/"), imageName);
                    if (Directory.Exists(Path.GetDirectoryName(file)))
                    {
                        System.IO.File.Delete(file);
                    }

                    // Remove slider cache
                    ObjectCache cache = MemoryCache.Default;
                    cache.Remove(ConstKey.ckHomeSlider);
                }
                else
                {
                    isSuccess = false;
                    ErrorLog.LogError("Delete Image Failed");
                }
            }

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }
    }
}