using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.ViewModel
{
    public class SliderImageViewModel
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public int DisplayOrder { get; set; }
        public string Url { get; set; }
    }
}