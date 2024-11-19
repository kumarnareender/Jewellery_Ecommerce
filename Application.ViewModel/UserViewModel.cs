using Application.Model.Models;
using System;
using System.Collections.Generic;

namespace Application.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int CustomerCode { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipZipCode { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipCountry { get; set; }
        public string MemberSince { get; set; }
        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public System.DateTime CreatDate { get; set; }
        public bool IsManual { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<bool> IsSync { get; set; }
        public string RoleNames { get; set; }
        public string BranchNames { get; set; }
        public string Permissions { get; set; }
        public List<Role> RoleList { get; set; }
        public List<Branch> BranchList { get; set; }
        public int IsWholeSaler { get; set; }
        public bool IsAdmin { get; set; }
    }
    public class CustomerViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string CustName { get; set; }
        public string CustEmail { get; set; }
        public string CustAddress { get; set; }
        public string CustPhone { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public string LogBy { get; set; }
        public DateTime Logtime { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}