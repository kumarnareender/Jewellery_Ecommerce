using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class ItemType
    {
        public ItemType()
        {
            this.Products = new List<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
