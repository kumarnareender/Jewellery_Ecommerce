using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{



    public class PromotionController : Controller
    {
        private IPromotionService promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            this.promotionService = promotionService;
        }

        public ActionResult Promotion()
        {
            return View();
        }

        public JsonResult GetPromotions(string name)
        {

            var itemList = this.promotionService.GetPromotionList();

            List<Promotions> list = new List<Promotions>();
            foreach (var item in itemList)
            {
                list.Add(new Promotions { Id = item.Id, Coupon = item.Coupon, StartDate = item.StartDate, EndDate = item.EndDate, Percentage = item.Percentage });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreatePromotion(Promotions promotions)
        {
            bool isSuccess = true;
            try
            {
                this.promotionService.CreatePromotion(promotions);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdatePromotion(Promotions promotions)
        {
            bool isSuccess = true;
            try
            {
                this.promotionService.UpdatePromotion(promotions);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePromotion(Promotions promotions)
        {
            bool isSuccess = true;
            try
            {
                this.promotionService.DeletePromotion(promotions);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}