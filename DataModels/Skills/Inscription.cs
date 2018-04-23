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

    public class Inscription
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Page")]
        public Guid PageId { get; set; }
        public Page Page { get; set; }

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
