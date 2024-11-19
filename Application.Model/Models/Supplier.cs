using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            this.Products = new List<Product>();
            this.Purchases = new List<Purchase>();
            this.PurchaseProductStocks = new List<PurchaseProductStock>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Contactperson { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<PurchaseProductStock> PurchaseProductStocks { get; set; }
    }
}
