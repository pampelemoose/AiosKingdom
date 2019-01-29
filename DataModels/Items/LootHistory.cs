using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Items
{
    public class LootHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid LooterId { get; set; }

        public Guid AdventureVid { get; set; }
        public Guid MonsterVid { get; set; }
        public Guid ItemVid { get; set; }
        public int Quantity { get; set; }

        public DateTime LootedAt { get; set; }
    }
}
