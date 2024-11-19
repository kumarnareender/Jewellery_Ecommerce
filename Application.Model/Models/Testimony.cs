using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class Testimony
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedById { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; }
        public bool IsActive { get; set; }

    }
}
