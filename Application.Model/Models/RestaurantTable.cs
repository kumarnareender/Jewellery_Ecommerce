using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Models
{
    public class RestaurantTable
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string ImageUrl { get; set; }
        public string OrderId { get; set; }
        public bool IsOccupied { get; set; }
    }
}
