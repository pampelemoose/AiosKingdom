using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Website
{
    public class Thread
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<Comment> Comments { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public bool IsAnnoucment { get; set; }

        [Required]
        public bool IsOpen { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public Category Category { get; set; }
    }
}
