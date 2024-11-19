using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class ProductImage
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ImageName { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public bool IsPrimaryImage { get; set; }
        public bool IsApproved { get; set; }
        public Nullable<System.DateTime> ActionDate { get; set; }
        public virtual Product Product { get; set; }
    }
}
