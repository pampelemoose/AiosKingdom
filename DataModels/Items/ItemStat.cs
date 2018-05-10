using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Items
{
    public class ItemStat
    {
        [Key]
        public int Id { get; set; }

        public Guid ItemId { get; set; }
        [ForeignKey("ItemId")]
        public AEquipableItem Item { get; set; }

        [Required]
        public Soul.Stats Type { get; set; }

        [Required]
        public int StatValue { get; set; }
    }
}
