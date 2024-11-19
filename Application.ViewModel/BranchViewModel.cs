using Application.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.ViewModel
{
    public class BranchViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAllowOnline { get; set; }
    }
}