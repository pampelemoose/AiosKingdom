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

        [Required]
        public Topic Category { get; set; }

        public List<Comment> Comments { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public string CreatedByUsername { get; set; }

        public DateTime? AssignedAt { get; set; }
        public Guid AssignedBy { get; set; }
        public string AssignedByUsername { get; set; }

        public Guid AssignedTo { get; set; }
        public string AssignedToUsername { get; set; }

        [Required]
        public bool IsOpen { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
