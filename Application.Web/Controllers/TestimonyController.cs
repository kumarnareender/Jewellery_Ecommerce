using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Application.Controllers
{
    //[Authorize]
    public class TestimonyController : Controller
    {
        private ITestimonyService testimonyService;

        public TestimonyController(ITestimonyService testimonyService)
        {
            this.testimonyService = testimonyService;
        }

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetAllTestimonies()
        {

            IEnumerable<Testimony> list = testimonyService.GetTestimonyList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveTestimonies()
        {
            IEnumerable<Testimony> list = testimonyService.GetActiveTestimonies();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateTestimony(Testimony testimony)
        {
            bool isSuccess = true;
            try
            {
                User user = Utils.GetLoggedInUser();
                testimony.CreatedByName = $"{user.FirstName} {user.LastName}";
                testimony.CreatedById = user.Id;
                testimony.CreatedOn = DateTime.Now;
                testimony.IsActive = false;

                testimonyService.CreateTestimony(testimony);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult UpdateTestimonies()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UpdateTestimonies(string testimoniesId)
        {
            bool isSuccess = true;
            try
            {
                string[] ids = testimoniesId.Split(',');

                List<Testimony> testimonies = testimonyService.GetTestimonyList();

                foreach (Testimony item in testimonies)
                {

                    if (ids.Contains(item.Id.ToString()))
                    {
                        item.IsActive = true;
                    }
                    else
                    {
                        item.IsActive = false;
                    }

                    testimonyService.UpdateTestimony(item);
                }


            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteTestimony(Testimony testimony)
        {
            bool isSuccess = true;
            try
            {
                testimonyService.DeleteTestimony(testimony);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}