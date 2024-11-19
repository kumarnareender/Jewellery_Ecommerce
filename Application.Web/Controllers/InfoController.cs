using System.Web.Mvc;

namespace Application.Web.Controllers
{
    public class InfoController : Controller
    {
        public ActionResult Message(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ActivationSuccess()
        {
            return View();
        }

        public ActionResult ActivationFailed()
        {
            return View();
        }        
	}
}