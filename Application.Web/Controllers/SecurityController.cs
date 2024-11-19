using Application.Common;
using Application.Logging;
using Application.Notification;
using Application.Service;
using Application.ViewModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace Application.Web.Controllers
{
    [Authorize]
    public class SecurityController : Controller
    {
        private IUserService userService;
        private readonly ISettingService settingService;

        public SecurityController(IUserService userService, ISettingService settingService)
        {
            this.userService = userService;
            this.settingService = settingService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View("Login");
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(LoginViewModel model)
        {
            bool isSuccess = false;
            string message = string.Empty;

            Model.Models.User user = userService.GetUser(model.Username);

            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                isSuccess = await SendPasswordToEmail(user.Email, user.Password);
            }
            else
            {
                message = "Your email not found!";
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            }, JsonRequestBehavior.AllowGet);

        }

        private async Task<bool> SendPasswordToEmail(string userName, string password)
        {
            EmailNotify emailNotify = new EmailNotify();

            string emailTemplate = Utils.GetFileText("~/Static/EmailTemplates/ForgotPassword.htm");

            string message = string.Empty;
            if (emailTemplate.Length > 0)
            {
                message = emailTemplate;
                message = message.Replace("#USERNAME#", userName);
                message = message.Replace("#PASSWORD#", password);
            }

            bool isSuccess = await emailNotify.SendEmail(userName, "Your password", message);
            return isSuccess;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            string redirectUrl = string.Empty;
            string message = string.Empty;
            bool isSuccess = false;

            if (ModelState.IsValid)
            {
                Model.Models.User user = userService.GetUser(model.Username, model.Password);
                if (user != null)
                {
                    isSuccess = true;

                    // Saving user info into session
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    System.Web.HttpContext.Current.Session["User"] = user;

                    // Update last login time
                    userService.UpdateLastLoginTime(user.Id);

                    if (string.IsNullOrEmpty(redirectUrl))
                    {
                        //if (userService.IsInRole(model.Username, ERoleName.customer.ToString()))
                        //{
                        //    redirectUrl = string.IsNullOrEmpty(model.ReturnUrl) ? Url.Action("Index", "Home") : model.ReturnUrl;
                        //}
                        //if (userService.IsInRole(model.Username, ERoleName.salesperson.ToString()))
                        //{
                        //    redirectUrl = string.IsNullOrEmpty(model.ReturnUrl) ? Url.Action("Index", "Home") : model.ReturnUrl;
                        //}
                        //else
                        //{
                        redirectUrl = Url.Action("Index", "Admin");
                        //}
                    }
                }
                else
                {
                    message = "Invalid username or password!";
                }
            }

            return Json(new
            {
                isSuccess = isSuccess,
                redirectUrl = redirectUrl,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        public void SignOut()
        {
            System.Web.HttpContext.Current.Session["User"] = null;
            FormsAuthentication.SignOut();
            Response.Redirect("/Home/Index");
        }

        [HttpPost]
        public ActionResult ChangePassword(string newPassword)
        {
            bool isSuccess = false;
            string message = string.Empty;

            string userName = User.Identity.Name;

            isSuccess = userService.ChangePassword(userName, newPassword);

            if (!isSuccess)
            {
                ErrorLog.LogError("Change Password Failed");
            }

            return Json(new
            {
                isSuccess = isSuccess,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult IsValidUser(string password)
        {
            bool isSuccess = false;
            string userName = User.Identity.Name;

            Model.Models.User user = userService.GetUser(userName, password);
            if (user != null)
            {
                isSuccess = true;
            }

            return Json(new
            {
                isSuccess = isSuccess
            }, JsonRequestBehavior.AllowGet);
        }
    }
}