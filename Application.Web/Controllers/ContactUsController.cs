using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    public class ContactUsController : Controller
    {

        private readonly IContactUsService _contactUsService;

        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        // GET: ContactUs
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetFeedbacks()
        {
            bool isSuccess = false;
            List<Feedback> data = null;
            try
            {
                data = _contactUsService.GetContactUsList();
                isSuccess = true;
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return Json(new Result { IsSuccess = isSuccess, Data = data }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddFeedback(Feedback feedback)
        {
            bool isSuccess = false;
            try
            {
                feedback.CreatedOn = DateTime.Now;
                _contactUsService.AddFeedback(feedback);
                isSuccess = true;
            }
            catch (Exception)
            {
                isSuccess = false;
            }


            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteFeed(int feedbackId)
        {
            bool isSuccess = true;
            try
            {
                Feedback feedback = _contactUsService.GetFeedback(feedbackId);
                _contactUsService.DeleteFeedback(feedback);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
    }
}