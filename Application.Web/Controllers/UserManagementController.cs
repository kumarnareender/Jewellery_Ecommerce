using Application.Model.Models;
using Application.Service;
using Application.ViewModel;
using Application.Web.App_Code;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web.Mvc;

namespace Application.Web.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private IUserService userService;
        private IRoleService roleService;
        private IBranchService branchService;
        private ISettingService settingService;
        private IActionLogService actionLogService;

        public UserManagementController(IUserService userService, IRoleService roleService, IBranchService branchService, ISettingService settingService, IActionLogService actionLogService)
        {
            this.userService = userService;
            this.roleService = roleService;
            this.branchService = branchService;
            this.settingService = settingService;
            this.actionLogService = actionLogService;
        }

        public ActionResult CreateUser()
        {            
            return View();
        }

        public ActionResult UserList()
        {
            return View();
        }

        public JsonResult GetUserList()
        {
            var userList = this.userService.GetManagementUsers();

            List<UserViewModel> userViewModelList = new List<UserViewModel>();
            UserViewModel userVM = null;

            foreach (User user in userList)
            {
                if (user.Username.ToLower() != "sadmin")
                {
                    userVM = new UserViewModel();
                    userVM.Id = user.Id;
                    userVM.Name = user.FirstName + " " + user.LastName;
                    userVM.FirstName = user.FirstName;
                    userVM.LastName = user.LastName;
                    userVM.Username = user.Username;
                    userVM.Password = user.Password;
                    userVM.Code = user.Code;
                    userVM.Mobile = user.Mobile;
                    userVM.Email = user.Email;
                    userVM.Permissions = user.Permissions;
                    userVM.CreatDate = user.CreateDate;
                    userVM.IsActive = user.IsActive != null ? (bool)user.IsActive : false;
                    userVM.RoleNames = GetUserRoleNames(user.Roles);
                    userVM.BranchNames = GetUserBranchNames(user.Branches);

                    userViewModelList.Add(userVM);
                }
            }

            return Json(userViewModelList, JsonRequestBehavior.AllowGet);
        }

        private string GetUserRoleNames(ICollection<Role> roles)
        {
            string roleNames = String.Empty;

            if (roles != null)
            {
                foreach (Role role in roles)
                {
                    roleNames += role.Name + ",";
                }

                roleNames = !String.IsNullOrEmpty(roleNames) ? roleNames.TrimEnd(',') : "";
            }

            return roleNames;
        }

        private string GetUserBranchNames(ICollection<Branch> branches)
        {
            string branchNames = String.Empty;

            if (branches != null)
            {
                foreach (Branch branch in branches)
                {
                    branchNames += branch.Name + ",";
                }

                branchNames = !String.IsNullOrEmpty(branchNames) ? branchNames.TrimEnd(',') : "";
            }

            return branchNames;
        }

        public JsonResult GetManagementRoles()
        {
            var roleList = this.roleService.GetManagementRoles();
            List<Role> roles = new List<Role>();

            foreach (var role in roleList)
            {
                roles.Add(new Role { Id = role.Id, Name = role.Name });
            }

            return Json(roles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUser(string userId)
        {            
            var user = this.userService.GetUserById(userId);
            UserViewModel userVM = new UserViewModel();
            
            if (user != null)
            {
                userVM.Name = user.FirstName;
                userVM.FirstName = user.FirstName;
                userVM.LastName = user.LastName;
                userVM.Username = user.Username;
                userVM.Password = user.Password;
                userVM.Mobile = user.Mobile;
                userVM.Email = user.Email;
                userVM.Code = user.Code;
                userVM.Permissions = user.Permissions;
                userVM.IsActive = user.IsActive;

                if (user.Roles != null)
                {
                    userVM.RoleList = new List<Role>();
                    foreach (Role role in user.Roles)
                    {
                        userVM.RoleList.Add(new Role { Id = role.Id, Name = role.Name });
                    }
                }

                if (user.Branches != null)
                {
                    userVM.BranchList = new List<Branch>();
                    foreach (Branch branch in user.Branches)
                    {
                        userVM.BranchList.Add(new Branch { Id = branch.Id, Name = branch.Name });
                    }
                }
            }

            return Json(userVM, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult Api_GetUser(string username, string password)
        {
            var user = this.userService.GetUser(username, password);
            UserViewModel userVM = new UserViewModel();

            if (user != null)
            {
                userVM.Id = user.Id;
                userVM.Name = user.FirstName;
                userVM.FirstName = user.FirstName;
                userVM.LastName = user.LastName;
                userVM.Username = user.Username;
                userVM.Password = user.Password;
                userVM.Mobile = user.Mobile;
                userVM.Email = user.Email;
                userVM.Code = user.Code;
                userVM.Permissions = user.Permissions;
                userVM.IsActive = user.IsActive;

                if (user.Roles != null)
                {
                    userVM.RoleList = new List<Role>();
                    foreach (Role role in user.Roles)
                    {
                        userVM.RoleList.Add(new Role { Id = role.Id, Name = role.Name });
                    }
                }

                if (user.Branches != null)
                {
                    userVM.BranchList = new List<Branch>();
                    foreach (Branch branch in user.Branches)
                    {
                        userVM.BranchList.Add(new Branch { Id = branch.Id, Name = branch.Name });
                    }
                }
            }

            return Json(userVM, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult Api_GetUserByCode(string code)
        {
            var user = this.userService.GetUserByCode(code);
            UserViewModel userVM = new UserViewModel();

            if (user != null)
            {
                userVM.Id = user.Id;
                userVM.Name = user.FirstName;
                userVM.FirstName = user.FirstName;
                userVM.LastName = user.LastName;
                userVM.Username = user.Username;
                userVM.Password = user.Password;
                userVM.Mobile = user.Mobile;
                userVM.Email = user.Email;
                userVM.Code = user.Code;
                userVM.Permissions = user.Permissions;
                userVM.IsActive = user.IsActive;

                if (user.Roles != null)
                {
                    userVM.RoleList = new List<Role>();
                    foreach (Role role in user.Roles)
                    {
                        userVM.RoleList.Add(new Role { Id = role.Id, Name = role.Name });
                    }
                }

                if (user.Branches != null)
                {
                    userVM.BranchList = new List<Branch>();
                    foreach (Branch branch in user.Branches)
                    {
                        userVM.BranchList.Add(new Branch { Id = branch.Id, Name = branch.Name });
                    }
                }
            }

            return Json(userVM, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult CreateUser(User user)
        {
            bool isSuccess = false;
            string message = String.Empty;

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (String.IsNullOrEmpty(user.Id)) // New User
            {
                using (TransactionScope tran = new TransactionScope())
                {
                    try
                    {
                        string userId = Guid.NewGuid().ToString();

                        // Check user is exists or not
                        var userExists = this.userService.GetUser(user.Username);
                        if (userExists != null)
                        {
                            message = "Username already Exists!";
                            return Json(new
                            {
                                isSuccess = isSuccess,
                                message = message
                            }, JsonRequestBehavior.AllowGet);
                        }

                        // If code provided then check duplicacy
                        if (!String.IsNullOrEmpty(user.Code))
                        {
                            var codeExists = this.userService.GetUserByCode(user.Code);
                            if (codeExists != null)
                            {
                                message = "User code is already Exists!";
                                return Json(new
                                {
                                    isSuccess = isSuccess,
                                    message = message
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        // Get all roles
                        var roleList = this.roleService.GetRoles();
                        List<Role> assignedRoles = new List<Role>();
                        foreach (Role role in roleList)
                        {
                            if (user.Roles != null)
                            {
                                foreach (var roleFromUI in user.Roles)
                                {
                                    if (role.Id == roleFromUI.Id)
                                    {
                                        assignedRoles.Add(role);
                                        break;
                                    }
                                }
                            }
                        }

                        // Get all branches
                        var branchList = this.branchService.GetBranchList();
                        List<Branch> assignedBranches = new List<Branch>();
                        foreach (Branch branch in branchList)
                        {
                            if (user.Branches != null)
                            {
                                foreach (var branchFromUI in user.Branches)
                                {
                                    if (branch.Id == branchFromUI.Id)
                                    {
                                        assignedBranches.Add(branch);
                                        break;
                                    }
                                }
                            }
                        }

                        user.Id = userId;
                        user.IsActive = user.IsActive;
                        user.IsManual = true;
                        user.CreateDate = DateTime.Now;                        

                        user.Roles = new List<Role>();
                        foreach (Role role in assignedRoles)
                        {
                            user.Roles.Add(role);
                        }

                        user.Branches = new List<Branch>();
                        foreach (Branch branch in assignedBranches)
                        {
                            user.Branches.Add(branch);
                        }

                        this.userService.CreateUser(user);
                        tran.Complete();
                        isSuccess = true;
                        AppCommon.WriteActionLog(actionLogService, "User Management", "User account created", "Username: " + user.Username, "Create", User.Identity.Name);
                    }
                    catch (Exception exp)
                    {
                        isSuccess = false;                        
                    }
                }

                return Json(new
                {
                    isSuccess = isSuccess                   
                }, JsonRequestBehavior.AllowGet);
            }
            else // Update User
            {          
                try
                {
                    var userReturn = this.userService.GetUserById(user.Id);
                    if (userReturn != null)
                    {

                        // If code provided then check duplicacy
                        if (!String.IsNullOrEmpty(user.Code))
                        {
                            var codeExists = this.userService.GetUserByCode(user.Code, user.Id);
                            if (codeExists != null)
                            {
                                message = "User code is already Exists!";
                                return Json(new
                                {
                                    isSuccess = isSuccess,
                                    message = message
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        
                        userReturn.FirstName = user.FirstName;
                        userReturn.LastName = user.LastName;
                        userReturn.Username = user.Username;
                        userReturn.Password = user.Password;
                        userReturn.Code = user.Code;
                        userReturn.Mobile = user.Mobile;
                        userReturn.Email = user.Email;
                        userReturn.Permissions = user.Permissions;
                        userReturn.IsActive = user.IsActive;                        

                        //----------------------------------- ROLES -----------------------------------------------
                        // Get all roles
                        var roleList = this.roleService.GetRoles();

                        List<Role> assignedRoles = new List<Role>();
                        foreach (Role role in roleList)
                        {
                            if (user.Roles != null)
                            {
                                foreach (var roleFromUI in user.Roles)
                                {
                                    if (role.Id == roleFromUI.Id)
                                    {
                                        assignedRoles.Add(role);
                                        break;
                                    }
                                }
                            }
                        }

                        // Getting current roles into generic list
                        List<Role> removeRoleList = new List<Role>();
                        foreach (var role in userReturn.Roles)
                        {
                            removeRoleList.Add(role);                            
                        }

                        // Remove previous roles
                        foreach (var role in removeRoleList)
                        {
                            userReturn.Roles.Remove(role);
                        }

                        // Adding new roles
                        foreach (var role in assignedRoles)
                        {
                            userReturn.Roles.Add(role);
                        }

                        //----------------------------------- BRANCHES -----------------------------------------------
                        // Get all roles
                        var branchList = this.branchService.GetBranchList();

                        List<Branch> assignedBranches = new List<Branch>();
                        foreach (Branch branch in branchList)
                        {
                            if (user.Branches != null)
                            {
                                foreach (var branchFromUI in user.Branches)
                                {
                                    if (branch.Id == branchFromUI.Id)
                                    {
                                        assignedBranches.Add(branch);
                                        break;
                                    }
                                }
                            }
                        }

                        // Getting current branches into generic list
                        List<Branch> removeBranchList = new List<Branch>();
                        foreach (var branch in userReturn.Branches)
                        {
                            removeBranchList.Add(branch);
                        }

                        // Remove previous branches
                        foreach (var branch in removeBranchList)
                        {
                            userReturn.Branches.Remove(branch);
                        }

                        // Adding new branches
                        foreach (var branch in assignedBranches)
                        {
                            userReturn.Branches.Add(branch);
                        } 
                    }

                    this.userService.UpdateUser(userReturn);
                    isSuccess = true;
                    ////AppCommon.WriteActionLog(actionLogService, EActionLogModule.General.ToString(), "User account updated", "Username: " + user.Username, EActionLogType.Update.ToString(), User.Identity.Name);
                }
                catch (Exception exp)
                {
                    isSuccess = false;
                }

                return Json(new
                {
                    isSuccess = isSuccess,                    
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteUser(string userId)
        {
            bool isSuccess = false;
            var user = this.userService.GetUserById(userId);
            if (user != null)
            {
                user.IsDelete = true;
                user.Username = user.Username + "_Deleted_" + DateTime.Now.Ticks.ToString();
                userService.UpdateUser(user);
                isSuccess = true;
            }

            return Json(new
            {
                isSuccess,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateAddress(string userId, string phone, string firstName, string lastName, string city, string prefecture, string postCode, string address,string mobile,string email)
        {
            bool isSuccess = false;
            var user = this.userService.GetUserById(userId);
            if (user != null)
            {
                user.Username = phone;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.ShipCity = city;
                user.Mobile = mobile;
                user.Email = email;
                user.ShipState = prefecture;
                user.ShipZipCode = postCode;
                user.ShipAddress = address;
                userService.UpdateUser(user);
                isSuccess = true;
            }

            return Json(new
            {
                isSuccess,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}