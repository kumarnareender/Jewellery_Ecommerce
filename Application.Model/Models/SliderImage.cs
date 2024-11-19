using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class SliderImage
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public string Url { get; set; }
    }
}
