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

        public Guid MonsterId { get; set; }

        public Guid ItemVid { get; set; }
        public double DropRate { get; set; }
        public int Quantity { get; set; }
    }
}
