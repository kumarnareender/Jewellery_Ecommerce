using Application.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.ViewModel
{
    public class CityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Postcode { get; set; }
        public decimal  ShippingCharge { get; set; }
        public bool IsAllowOnline { get; set; }
    }
}