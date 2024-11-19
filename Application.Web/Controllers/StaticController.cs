using Application.Common;
using Application.Notification;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class StaticController : Controller
    {
        public StaticController()
        {
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult CookiePolicy()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ContactUs(string name, string phone, string message)
        {
            string companyEmail = Utils.GetSetting(ESetting.CompanyEmail.ToString());
            string companyPhone = Utils.GetSetting(ESetting.CompanyPhone.ToString());

            EmailNotify emailNotify = new EmailNotify();

            string body = "Name: " + name + " Phone: " + phone + " Message: " + message;

            bool isSuccess = await emailNotify.SendEmail(companyEmail, "Message from ecommerce website", body);

            return Json(new
            {
                isSuccess = isSuccess,
                hotlineNumber = companyPhone
            }, JsonRequestBehavior.AllowGet);
        }

    }
}