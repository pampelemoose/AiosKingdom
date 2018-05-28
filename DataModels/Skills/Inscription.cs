using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Skills
{
    public enum InscriptionType
    {
        Damages = 0,
        Heal = 1
    }

    public enum ElementType
    {
        Neutral = 0,

        Fire = 1,
        Water = 2,
        Wind = 3,
        Earth = 4,
        Lightning = 5,
        
        // ADVANCED IDEAS
        Light = 6,
        Shadow = 7,

        Ice = 8,
        Magma = 9,

        Life = 10,
        Death = 11
    }

    public class Inscription
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PageId { get; set; }

        [Required, MaxLength(50)]
        public string Description { get; set; }

        [Required]
        public InscriptionType Type { get; set; }

        [Required]
        public int BaseValue { get; set; }

        [Required]
        public Soul.Stats StatType { get; set; }

        [Required]
        public float Ratio { get; set; }

        [Required]
        public int Duration { get; set; }
    }
}
