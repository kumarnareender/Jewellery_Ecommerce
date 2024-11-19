using Application.Common;
using Application.Logging;
using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;

namespace Application.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IRoleService roleService;
        private IUserService userService;

        public AccountController(IUserService userService, IRoleService roleService)
        {
            this.roleService = roleService;
            this.userService = userService;
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register1()
        {
            return View("Register");
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        public ActionResult AccountInfo()
        {
            return View();
        }

        public ActionResult ChangeUsername()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RegisterAccount(User user, bool byAdmin = false)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess = true;
                string userId = Guid.NewGuid().ToString();

                if (user.IsWholeSaler)
                {
                    user.Username = user.FirstName + user.LastName;
                    user.Password = string.Empty;
                }

                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(user.Id)) // Create
                        {
                            // Check username is already taken or not
                            User userExist = userService.GetUser(user.Username);
                            if (userExist != null)
                            {
                                return Json(new
                                {
                                    isSuccess = false,
                                    message = "This mobile is already used as username. Please try with another mobile! ",
                                }, JsonRequestBehavior.AllowGet);
                            }

                            // Get member role
                            Role memberRole = roleService.GetRole(ERoleName.customer.ToString());
                            if (memberRole == null)
                            {
                                isSuccess = false;
                                ErrorLog.LogError("Member Role is null");
                            }

                            if (isSuccess)
                            {
                                user.Id = userId;
                                user.IsActive = true;
                                user.IsVerified = true;
                                user.IsDelete = false;
                                user.CreateDate = DateTime.Now;
                                user.Roles.Add(memberRole);
                                userService.CreateUser(user);

                                if (!byAdmin)
                                {
                                    // Silent login
                                    SilentLogin(userId);

                                    // Store the userid in cookie
                                    Utils.SetCookie("userid", userId);
                                }

                                // Now complete the transaction
                                tran.Complete();
                            }
                        }
                        else // Update
                        {
                            // Check username is already taken or not
                            User userExist = userService.GetUserExcludeMe(user.Id, user.Username);
                            if (userExist != null)
                            {
                                return Json(new
                                {
                                    isSuccess = false,
                                    message = "This mobile is already used as username. Please try with another mobile! ",
                                }, JsonRequestBehavior.AllowGet);
                            }

                            User userToUpdate = userService.GetUserById(user.Id);
                            if (userToUpdate != null)
                            {
                                userToUpdate.Username = user.Username;
                                userToUpdate.Password = user.Password;
                                userToUpdate.FirstName = user.FirstName;
                                userToUpdate.LastName = user.LastName;
                                userToUpdate.Mobile = user.Mobile;
                                userToUpdate.Email = user.Email;
                                userToUpdate.ShipAddress = user.ShipAddress;
                                userToUpdate.ShipZipCode = user.ShipZipCode;
                                userToUpdate.ShipCity = user.ShipCity;
                                userToUpdate.ShipState = user.ShipState;
                                userToUpdate.ShipCountry = user.ShipCountry;

                                userService.UpdateUser(userToUpdate);
                                tran.Complete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        ErrorLog.LogError(ex);
                    }
                }

                return Json(new
                {
                    isSuccess = isSuccess,
                    userId = userId,
                }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateUserAddress(string mobile, string firstName, string lastName, string address, string zipCode, string city, string state, string country, string email)
        {
            bool isSuccess = false;
            User user = userService.GetUserById(Utils.GetLoggedInUserId());
            if (user != null)
            {
                // Check username is already taken or not
                User userExist = userService.GetUserExcludeMe(user.Id, mobile);
                if (userExist != null)
                {
                    return Json(new
                    {
                        isSuccess = false,
                        message = "This mobile is already used as username. Please try with another mobile! ",
                    }, JsonRequestBehavior.AllowGet);
                }

                user.Mobile = mobile;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.ShipAddress = address;
                user.ShipZipCode = zipCode;
                user.ShipCity = city;
                user.ShipState = state;
                user.ShipCountry = country;
                user.Email = email;
                userService.UpdateUserInfo(user);
                isSuccess = true;
            }

            if (!isSuccess)
            {
                ErrorLog.LogError("User Info Update Failed!");
            }

            return Json(new
            {
                isSuccess = isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateUserInfo(string email)
        {
            bool isSuccess = false;
            User user = userService.GetUserById(Utils.GetLoggedInUserId());
            if (user != null)
            {
                user.Username = email;

                userService.UpdateUserInfo(user);
                isSuccess = true;
            }

            if (!isSuccess)
            {
                ErrorLog.LogError("User Info Update Failed!");
            }

            return Json(new
            {
                isSuccess = isSuccess
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetUserStatus()
        {
            bool isUserLoggedIn = false;
            bool isUserVerified = false;
            bool isAdmin = User.IsInRole(ERoleName.admin.ToString());

            isUserLoggedIn = (Utils.GetLoggedInUser() == null) ? false : true;

            if (isUserLoggedIn)
            {
                isUserVerified = Utils.GetLoggedInUser().IsVerified;
            }

            return Json(new
            {
                isLoggedIn = isUserLoggedIn,
                isVerified = isUserVerified,
                isAdmin = isAdmin
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetUser(string username)
        {
            User user = userService.GetUser(username);
            return Json(username, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByPhone(string phoneNo)
        {
            UserViewModel uvm = new UserViewModel();

            User user = userService.GetUserByPhone(phoneNo);

            if (user != null)
            {
                uvm.Id = user.Id;
                uvm.Name = user.FirstName + " " + user.LastName;
                uvm.Username = user.Username;
                uvm.FirstName = user.FirstName;
                uvm.LastName = user.LastName;
                uvm.Mobile = user.Mobile;
                uvm.Email = user.Email;
                uvm.ShipAddress = user.ShipAddress;
                uvm.ShipCity = user.ShipCity;
                uvm.ShipCountry = user.ShipCountry;
                uvm.ShipState = user.ShipState;
                uvm.ShipZipCode = user.ShipZipCode;
            }

            return Json(uvm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLoggedInUserInformation()
        {
            UserViewModel uvm = new UserViewModel();

            User user = Utils.GetLoggedInUser();
            if (user != null)
            {
                uvm.Name = user.FirstName + " " + user.LastName;
                uvm.Username = user.Username;
                uvm.FirstName = user.FirstName;
                uvm.LastName = user.LastName;
                uvm.ShipAddress = user.ShipAddress;
                uvm.Mobile = user.Mobile;
                uvm.Email = user.Email;
                uvm.ShipCity = user.ShipCity;
                uvm.ShipCountry = user.ShipCountry;
                uvm.ShipState = user.ShipState;
                uvm.ShipZipCode = user.ShipZipCode;
            }

            return Json(uvm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLoggedInUserAddress()
        {
            UserViewModel uvm = new UserViewModel();

            User user = Utils.GetLoggedInUser();
            if (user != null)
            {
                user = userService.GetUser(user.Username);

                uvm.Name = user.FirstName + " " + user.LastName;
                uvm.Username = user.Username;
                uvm.FirstName = user.FirstName;
                uvm.LastName = user.LastName;
                uvm.Mobile = user.Mobile;
                uvm.Email = user.Email;
                uvm.ShipAddress = user.ShipAddress;
                uvm.ShipCity = user.ShipCity;
                uvm.ShipCountry = user.ShipCountry;
                uvm.ShipState = user.ShipState;
                uvm.ShipZipCode = user.ShipZipCode;

                if (user.Roles.Where(x => x.Name == "admin").Any())
                {
                    uvm.IsAdmin = true;
                }
            }

            return Json(uvm, JsonRequestBehavior.AllowGet);
        }

        private void SilentLogin(string userId)
        {
            User user = userService.GetUserById(userId);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Username, true);
                Session["User"] = user;
            }
            else
            {
                ErrorLog.LogError("SilentLogin(): User is null");
            }
        }

    }
}