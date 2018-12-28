using DataModels;
using DataModels.Skills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class BookModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quality required")]
        [Display(Name = "Quality")]
        public DataModels.Skills.BookQuality Quality { get; set; }

        [Required(ErrorMessage = "StatCost required")]
        [Display(Name = "StatCost")]
        [Range(1, 10000, ErrorMessage = "StatCost should be higher than 0")]
        public int StatCost { get; set; }

        [Required(ErrorMessage = "ManaCost required")]
        [Display(Name = "ManaCost")]
        [Range(0, 400)]
        public int ManaCost { get; set; }

        [Required(ErrorMessage = "Cooldown required")]
        [Display(Name = "Cooldown")]
        [Range(0, 400)]
        public int Cooldown { get; set; }

        [Display(Name = "New Insc")]
        [Range(0, 1000000)]
        public int NewInsc { get; set; }

        [Display(Name = "Inscriptions")]
        public List<InscriptionModel> Inscriptions { get; set; }

        [Display(Name = "Talents")]
        public List<TalentModel> Talents { get; set; }
    }

    public class InscriptionModel
    {
        public Guid Id { get; set; }
        public string BookId { get; set; }

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

    public class InscWeaponTypeModel
    {
        public string InscId { get; set; }
        public string TypeExtension { get; set; }

        public DataModels.Items.ItemType? Type { get; set; }
    }

    public class TalentModel
    {
        public Guid Id { get; set; }

        [Required]
        [Range(0, 11, ErrorMessage = "Branch should be between 0 and 11.")]
        public int Branch { get; set; }

        [Required]
        [Range(0, 29, ErrorMessage = "Leaf should be between 0 and 29.")]
        public int Leaf { get; set; }

        public List<TalentUnlockTypeModel> Unlocks { get; set; }

        [Required]
        public Guid TargetInscription { get; set; }

        public List<DataModels.Skills.Inscription> Inscriptions { get; set; }

        [Required]
        [Range(1, 10000000, ErrorMessage = "TalentPointsRequired should be at least > 0")]
        public int TalentPointsRequired { get; set; }

        [Required]
        public TalentType Type { get; set; }

        [Required]
        [Range(0.00001, 100000000, ErrorMessage = "Duration should be > 0")]
        public double Value { get; set; }
    }

    public class TalentUnlockTypeModel
    {
        public string TalentId { get; set; }

        private DataModels.Skills.TalentUnlock _type = TalentUnlock.None;
        public DataModels.Skills.TalentUnlock Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}