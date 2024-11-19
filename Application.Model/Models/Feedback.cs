using System;

namespace Application.Model.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
