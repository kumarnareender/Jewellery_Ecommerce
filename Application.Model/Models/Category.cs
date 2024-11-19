using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class Category
    {
        public Category()
        {
            this.Category1 = new List<Category>();
            this.Products = new List<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public Nullable<bool> ShowInHomepage { get; set; }
        public string ImageName { get; set; }
        public virtual ICollection<Category> Category1 { get; set; }
        public virtual Category Category2 { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
