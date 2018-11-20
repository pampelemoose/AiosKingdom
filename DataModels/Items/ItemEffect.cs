using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Items
{
    public enum EffectType
    {
        RestoreHealth = 0,
        ResoreMana = 1,

        IncreaseStamina = 10,
        IncreaseEnergy = 11,
        IncreaseStrength = 12,
        IncreaseAgility = 13,
        IncreaseIntelligence = 14,
        IncreaseWisdom = 15
    }

    public class ItemEffect
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }

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
