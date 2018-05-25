using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Knowledge
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SoulId { get; set; }

        [Required]
        public Guid BookId { get; set; }

        [Required]
        public int Rank { get; set; }
    }
}
