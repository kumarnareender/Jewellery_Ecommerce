using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.ViewModel
{
    public class SearchModel
    {
        public string FreeText { get; set; }
        public string OnlyDiscount { get; set; }
        public string CategoryId { get; set; }
        public string LocationId { get; set; }
        public string Condition { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int PageNo { get; set; }
        public int TotalPages { get; set; }
        public bool IsGetTotalRecord { get; set; }
        public string SellType { get; set; }
        public string SortOrder { get; set; }
    }
}