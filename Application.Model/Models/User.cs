using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class User
    {
        public User()
        {
            this.Orders = new List<Order>();
            this.Products = new List<Product>();
            this.Branches = new List<Branch>();
            this.Roles = new List<Role>();
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string ShipAddress { get; set; }
        public string ShipZipCode { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipCountry { get; set; }
        public string PhotoUrl { get; set; }
        public Nullable<System.DateTime> LastLoginTime { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<bool> IsManual { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public Nullable<bool> IsSync { get; set; }
        public string Code { get; set; }
        public string Permissions { get; set; }
        public int CustomerCode { get; set; }
        public bool IsWholeSaler { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
