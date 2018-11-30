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

        public string Name { get; set; }

        public List<Comment> Comments { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByUsername { get; set; }
        public bool IsAnnoucment { get; set; }
        public bool IsOpen { get; set; }
        public bool IsActive { get; set; }

        public Category Category { get; set; }
    }
}
