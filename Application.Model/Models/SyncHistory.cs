using System;
using System.Collections.Generic;

namespace Application.Model.Models
{
    public partial class SyncHistory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> ActionDate { get; set; }
    }
}
