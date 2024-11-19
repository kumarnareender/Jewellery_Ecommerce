using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Model.Models
{
    public partial class SupplierPayment
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Purchase Purchase { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }

    }
}
