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

        public string Name { get; set; }
        public string Description { get; set; }
        public EffectType Type { get; set; }
        public float AffectValue { get; set; }
        public int AffectTime { get; set; }
    }
}
