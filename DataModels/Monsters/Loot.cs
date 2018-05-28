using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Monsters
{
    public class Loot
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid MonsterId { get; set; }

        [Required]
        [Range(0, 100)]
        public double DropRate { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
    }
}
