using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Models
{
    public partial class OrderStatus
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public int OldStatusId { get; set; }
        public int NewStatusId { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
