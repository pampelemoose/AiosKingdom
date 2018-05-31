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
        [Range(0, 10000, ErrorMessage = "BaseValue should be >= 0.")]
        public int BaseValue { get; set; }

        [Required]
        public Soul.Stats StatType { get; set; }

        [Required]
        [Range(0.0001, 10000, ErrorMessage = "Ratio should be at least > 0")]
        public float Ratio { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Duration should be >= 0")]
        public int Duration { get; set; }
    }
}
