using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class SupplierController : Controller
    {
        private ISupplierService supplierService;
        private readonly ICityService cityService;

        public SupplierController(ISupplierService supplierService, ICityService cityService)
        {
            this.supplierService = supplierService;
            this.cityService = cityService;
        }

        public ActionResult Supplier()
        {
            return View();
        }

        public JsonResult GetSupplierList()
        {
            IEnumerable<Supplier> itemList = supplierService.GetSupplierList();

            List<Supplier> list = new List<Supplier>();
            foreach (Supplier item in itemList)
            {
                list.Add(new Supplier
                {
                    Id = item.Id,
                    Name = item.Name,
                    Address = item.Address,
                    Mobile = item.Mobile,
                    City = item.City,
                    Contactperson = item.Contactperson,
                    Notes = item.Notes,
                    Phone = item.Phone,
                    Postcode = item.Postcode,
                    State = item.State,
                    Email = item.Email

                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateSupplier(Supplier supplier)
        {
            bool isSuccess = true;
            try
            {
                AddPostCode(supplier);
                supplierService.CreateSupplier(supplier);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSupplier(Supplier supplier)
        {
            bool isSuccess = true;
            try
            {
                AddPostCode(supplier);
                supplierService.UpdateSupplier(supplier);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteSupplier(Supplier supplier)
        {
            bool isSuccess = true;
            try
            {
                supplierService.DeleteSupplier(supplier);
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        private void AddPostCode(Supplier supplier)
        {
            City city = cityService.GetCityByName(supplier.City);
            if (city != null)
            {
                supplier.Postcode = city.Postcode;
            }

        }

    }
}