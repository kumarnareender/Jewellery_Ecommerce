using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class Branch
    {
        public Branch()
        {
            this.Orders = new List<Order>();
            this.Products = new List<Product>();
            this.Users = new List<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAllowOnline { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
