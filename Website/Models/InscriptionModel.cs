using DataModels;
using DataModels.Skills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class InscriptionModel
    {
        public Guid Id { get; set; }

        public string PageId { get; set; }

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

        public bool IncludeWeaponDamages { get; set; }
        public List<InscWeaponTypeModel> WeaponTypes { get; set; }
        public float WeaponDamagesRatio { get; set; }
        public List<InscWeaponTypeModel> PreferredWeaponTypes { get; set; }
        public float PreferredWeaponDamagesRatio { get; set; }
    }
}