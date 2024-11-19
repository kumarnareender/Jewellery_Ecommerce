using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class LookupController : Controller
    {
        private ILookupService lookupService;

        public LookupController(ILookupService lookupService)
        {
            this.lookupService = lookupService;            
        }

        public ActionResult Lookup()     
        {           
            return View();
        }

        public JsonResult GetLookups(string name)
        {
            var itemList = this.lookupService.GetLookupList(name);

            List<Lookup> list = new List<Lookup>();
            foreach (var item in itemList)
            {
                list.Add(new Lookup { Id = item.Id, Name = item.Name, Value = item.Value});
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateLookup(Lookup lookup)
        {
            bool isSuccess = true;
            try
            {
                this.lookupService.CreateLookup(lookup);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateLookup(Lookup lookup)
        {
            bool isSuccess = true;
            try
            {
                this.lookupService.UpdateLookup(lookup);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteLookup(Lookup lookup)
        {
            bool isSuccess = true;
            try
            {
                this.lookupService.DeleteLookup(lookup);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }        
                    
    }
}