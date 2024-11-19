using System;
using System.Collections.Generic;

namespace Application.ViewModel
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string ParentName { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public Nullable<bool> ShowInHomepage { get; set; }
        public string ImageName { get; set; }
        public bool HasChild { get; set; }
        public List<CategoryViewModel> ChildCategories { get; set; }        
    }
}
