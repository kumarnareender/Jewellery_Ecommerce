using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class AboutUsController : Controller
    {
        private IAboutUsService aboutusService;

        public AboutUsController(IAboutUsService aboutusService)
        {
            this.aboutusService = aboutusService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAboutUs(string name)
        {

            System.Collections.Generic.List<AboutUs> itemList = aboutusService.GetAboutUsList();

            //List<AboutUs> list = new List<AboutUs>();
            //foreach (var item in itemList)
            //{
            //    list.Add(new AboutUs { Id = item.Id, Description = item.Description,Images=item.Images });
            //}

            return Json(itemList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CreateAboutUs()
        {
            return View();
        }
        [HttpPost]
        public JsonResult CreateAboutUs(AboutUs aboutus)
        {
            bool isSuccess = true;
            try
            {
                if (aboutus.Id > 0)
                {
                    aboutusService.UpdateAboutUs(aboutus);
                }
                else
                {
                    aboutus.Images = "";
                    aboutusService.CreateAboutUs(aboutus);
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateAboutUs(AboutUs aboutus)
        {
            bool isSuccess = true;
            try
            {
                aboutusService.UpdateAboutUs(aboutus);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAboutUs(AboutUs aboutus)
        {
            bool isSuccess = true;
            try
            {
                aboutusService.DeleteAboutUs(aboutus);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePhoto()
        {
            bool isSuccess = false;
            //string message = String.Empty;

            if (Request.Files == null || Request.Files[0] == null)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Please choose a user image!"
                }, JsonRequestBehavior.AllowGet);
            }


            // Check max limit reached
            AboutUs aboutUs = aboutusService.GetAboutUsList().FirstOrDefault();
            string[] images = aboutUs.Images.Split(',');

            if (images.Length >= 3)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "About us image upload limit exceeded! Please delete a existing image to add a new image"
                }, JsonRequestBehavior.AllowGet);
            }
            if (aboutUs.Images == "")
            {
                aboutUs.Images += UploadImage(Request);
            }
            else
            {
                aboutUs.Images += ',' + UploadImage(Request);
            }
            aboutusService.UpdateAboutUs(aboutUs);
            isSuccess = true;
            //////bool isPrimaryImage = photoList.Count() == 0 ? true : false;
            //isSuccess = AppUtils.SaveProductImage(productImageService, Request, productId, false, isPrimaryImage);

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult DeletePhoto(string imageId)
        {
            bool isSuccess = true;


            AboutUs aboutUs = aboutusService.GetAboutUsList().FirstOrDefault();
            aboutUs.Images = aboutUs.Images.Replace(imageId, "");

            string images = string.Empty;
            foreach (string image in aboutUs.Images.Split(','))
            {
                if (image != "")
                {
                    images += image + ',';
                }
            }

            if (images.Length != 0)
            {
                StringBuilder sb = new StringBuilder(images);
                sb[images.Length - 1] = ' ';
                aboutUs.Images = sb.ToString();
            }
            else
            {
                aboutUs.Images = images;
            }
            aboutusService.UpdateAboutUs(aboutUs);
            isSuccess = DeleteAboutUsImage(imageId);

            return Json(new
            {
                isSuccess
            }, JsonRequestBehavior.AllowGet);
        }
        private string UploadImage(HttpRequestBase request)
        {
            string fileName = string.Empty;
            foreach (string name in request.Files)
            {
                HttpPostedFileBase file = request.Files[name];

                string originalFileName = file.FileName;
                string fileExtension = Path.GetExtension(originalFileName);
                string aboutUsImageId = Guid.NewGuid().ToString();

                fileName = aboutUsImageId + ".jpg";
                string imagePath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ProductImages/AboutUs/"), fileName);

                // Save photo
                file.SaveAs(imagePath);

                // Saving photo in different sizes
                string imageSource = string.Empty;
                string imageDest = string.Empty;

                // Small
                //imageSource = imagePath;
                //imageDest = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ProductImages/Small/"), fileName);
                //ImageResizer.Resize_AspectRatio(imageDest, imageSource, 140, 140);

                // Medium
                //imageSource = imagePath;
                //imageDest = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ProductImages/AboutUs/"), fileName);
                //ImageResizer.Resize_AspectRatio(imageDest, imageSource, 200, 200);

                // Large
                //imageSource = imagePath;
                //imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Large/"), fileName);
                //ImageResizer.Resize_AspectRatio(imageDest, imageSource, 650, 650);

                // XLarge
                //imageSource = imagePath;
                //imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/XLarge/"), fileName);
                //ImageResizer.Resize_AspectRatio(imageDest, imageSource, 800, 800);

                // Home page grid
                //imageSource = imagePath;
                //imageDest = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Grid/"), fileName);
                //ImageResizer.Resize_AspectRatio(imageDest, imageSource, 250, 250);

                // Save records to db
            }
            return fileName;
        }
        private bool DeleteAboutUsImage(string imageId)
        {
            bool isSuccess = false;

            //if (isSuccess)
            //{
            // Delete from file
            string file = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/ProductImages/AboutUs/"), imageId);
            if (Directory.Exists(Path.GetDirectoryName(file)))
            {
                System.IO.File.Delete(file);
            }
            isSuccess = true;
            //file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Large/"), productImage.ImageName);
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}

            //file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Medium/"), productImage.ImageName);
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}

            //file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Original/"), productImage.ImageName);
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}

            //file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Small/"), productImage.ImageName);
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}

            //file = Path.Combine(HttpContext.Current.Server.MapPath("~/ProductImages/Grid/"), productImage.ImageName);
            //if (Directory.Exists(Path.GetDirectoryName(file)))
            //{
            //    System.IO.File.Delete(file);
            //}
            //}
            //else
            //{
            //    isSuccess = false;
            //    ErrorLog.LogError("Delete Image Failed");
            //}


            return isSuccess;
        }


    }
}