using Application.Common;
using Application.Model.Models;
using Application.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Application.Controllers
{
    public class BranchController : Controller
    {
        private IBranchService branchService;

        public BranchController(IBranchService branchService)
        {
            this.branchService = branchService;            
        }

        public ActionResult Branch()     
        {           
            return View();
        }

        public JsonResult GetBranchList()
        {
            var itemList = this.branchService.GetBranchList();

            List<Branch> list = new List<Branch>();
            foreach (var item in itemList)
            {
                list.Add(new Branch { Id = item.Id, Name = item.Name, IsAllowOnline = item.IsAllowOnline });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserBranchList()
        {
            var user = Utils.GetLoggedInUser();
            var itemList = this.branchService.GetBranchList(user.Id);
            
            List<Branch> list = new List<Branch>();
            foreach (var item in itemList)
            {
                list.Add(new Branch { Id = item.Id, Name = item.Name, IsAllowOnline = item.IsAllowOnline });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateBranch(Branch branch)
        {
            bool isSuccess = true;
            try
            {
                this.branchService.CreateBranch(branch);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateBranch(Branch branch)
        {
            bool isSuccess = true;
            try
            {
                this.branchService.UpdateBranch(branch);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteBranch(Branch branch)
        {
            bool isSuccess = true;
            try
            {
                this.branchService.DeleteBranch(branch);
            }
            catch (Exception exp)
            {
                isSuccess = false;
            }

            return Json(new Result { IsSuccess = isSuccess }, JsonRequestBehavior.AllowGet);
        }        
                    
    }
}