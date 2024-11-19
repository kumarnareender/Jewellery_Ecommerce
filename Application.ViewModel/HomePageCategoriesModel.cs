using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.ViewModel
{
    public class HomePageCategoriesModel
    {
        public string CategoryId { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public List<HomePageItem> ProductList { get; set; }
    }
}