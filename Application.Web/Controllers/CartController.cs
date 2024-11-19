using System.Web.Mvc;

namespace Application.Controllers
{
    public class CartController : Controller
    {
        public CartController()
        {

        }

        public ActionResult Index()
        {
            return View();
        }
    }
}