using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Website
{
    public class Ticket
    {
        public enum Topic
        {
            EmailChange = 0
        }

        [Key]
        public int Id { get; set; }

        public Topic Category { get; set; }

        public List<Comment> Comments { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByUsername { get; set; }

        public DateTime? AssignedAt { get; set; }
        public Guid AssignedBy { get; set; }
        public string AssignedByUsername { get; set; }

        public Guid AssignedTo { get; set; }
        public string AssignedToUsername { get; set; }

        public bool IsOpen { get; set; }
        public bool IsActive { get; set; }
    }
}
