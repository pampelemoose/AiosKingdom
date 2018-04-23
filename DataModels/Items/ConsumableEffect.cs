using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Items
{
    public enum EffectType
    {
        RestoreHealth = 0
    }

    public class ConsumableEffect
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Consumable")]
        public Guid ConsumableId { get; set; }
        public Consumable Consumable { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public EffectType Type { get; set; }

        [Required]
        public float AffectValue { get; set; }

        [Required]
        public int AffectTime { get; set; }
    }
}
