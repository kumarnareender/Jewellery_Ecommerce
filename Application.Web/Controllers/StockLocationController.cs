using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class StockLocationController : Controller
    {
        private IStockLocationService stockLocationService;

        public StockLocationController(IStockLocationService stockLocationService)
        {
            this.stockLocationService = stockLocationService;            
        }

        public ActionResult StockLocation()     
        {           
            return View();
        }

        public JsonResult GetStockLocationList()
        {
            var itemList = this.stockLocationService.GetStockLocationList();

            List<StockLocation> list = new List<StockLocation>();
            foreach (var item in itemList)
            {
                list.Add(new StockLocation { Id = item.Id, Name = item.Name });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateStockLocation(StockLocation stockLocation)
        {
            bool isSuccess = true;
            try
            {
                this.stockLocationService.CreateStockLocation(stockLocation);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateStockLocation(StockLocation stockLocation)
        {
            bool isSuccess = true;
            try
            {
                this.stockLocationService.UpdateStockLocation(stockLocation);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteStockLocation(StockLocation stockLocation)
        {
            bool isSuccess = true;
            try
            {
                this.stockLocationService.DeleteStockLocation(stockLocation);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }        
                    
    }
}