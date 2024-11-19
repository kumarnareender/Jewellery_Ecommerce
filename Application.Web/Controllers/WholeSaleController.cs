using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    public class WholeSaleController : Controller
    {
        // GET: WholeSale
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult OrderConfirm()
        {
            return View();
        }
    }
}