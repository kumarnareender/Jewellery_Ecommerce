using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class CityController : Controller
    {
        private ICityService cityService;

        public CityController(ICityService cityService)
        {
            this.cityService = cityService;
        }

        public ActionResult City()
        {
            return View();
        }

        public JsonResult GetCities(string name)
        {

            var itemList = this.cityService.GetCityList();

            List<City> list = new List<City>();
            foreach (var item in itemList)
            {
                list.Add(new City { Id = item.Id, Name = item.Name, Postcode = item.Postcode, ShippingCharge = item.ShippingCharge, IsAllowOnline = item.IsAllowOnline });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateCity(City city)
        {
            bool isSuccess = true;
            try
            {
                this.cityService.CreateCity(city);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCity(City city)
        {
            bool isSuccess = true;
            try
            {
                this.cityService.UpdateCity(city);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCity(City city)
        {
            bool isSuccess = true;
            try
            {
                this.cityService.DeleteCity(city);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}