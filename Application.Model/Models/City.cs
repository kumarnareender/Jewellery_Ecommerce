using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Postcode { get; set; }
        public decimal ShippingCharge { get; set; }
        public bool IsAllowOnline { get; set; }
       
    }
}
