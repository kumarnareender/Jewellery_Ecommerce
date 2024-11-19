using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.ViewModel
{
    public class ProductImageViewModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ImageName { get; set; }
        public string ThumbImageName { get; set; }
        public string MaxViewImageName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
        public bool IsPrimaryImage { get; set; }
        public DateTime ActionDate { get; set; }
    }
}