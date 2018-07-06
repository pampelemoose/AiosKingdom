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

        [Required]
        public bool IncludeWeaponDamages { get; set; }

        [Required]
        public string InternalWeaponTypes { get; set; }
        public List<Items.WeaponType> WeaponTypes
        {
            get
            {
                var result = new List<Items.WeaponType>();
                foreach (var str in InternalWeaponTypes.Split(';'))
                {
                    if (!String.IsNullOrEmpty(str))
                        result.Add((Items.WeaponType)Enum.Parse(typeof(Items.WeaponType), str));
                }
                return result;
            }
            set
            {
                InternalWeaponTypes = String.Join(";", value);
            }
        }

        [Required]
        [Range(0.0001, 10000, ErrorMessage = "Ratio should be at least > 0")]
        public float WeaponDamagesRatio { get; set; }

        [Required]
        public string InternalPreferredWeaponTypes { get; set; }
        public List<Items.WeaponType> PreferredWeaponTypes
        {
            get
            {
                var result = new List<Items.WeaponType>();
                foreach (var str in InternalPreferredWeaponTypes.Split(';'))
                {
                    if (!String.IsNullOrEmpty(str))
                        result.Add((Items.WeaponType)Enum.Parse(typeof(Items.WeaponType), str));
                }
                return result;
            }
            set
            {
                InternalPreferredWeaponTypes = String.Join(";", value);
            }
        }

        [Required]
        [Range(0.0001, 10000, ErrorMessage = "Ratio should be at least > 0")]
        public float PreferredWeaponDamagesRatio { get; set; }

    }
}
