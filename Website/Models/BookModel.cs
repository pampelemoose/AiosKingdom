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

        [Required(ErrorMessage = "Quality required")]
        [Display(Name = "Quality")]
        public DataModels.Skills.BookQuality Quality { get; set; }

        [Display(Name = "New Pages")]
        [Range(0, 1000000)]
        public int NewPages { get; set; }

        [Display(Name = "Pages")]
        public List<PageModel> Pages { get; set; }
    }

    public class PageModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "StatCost required")]
        [Display(Name = "StatCost")]
        [Range(1, 10000, ErrorMessage = "StatCost should be higher than 0")]
        public int StatCost { get; set; }

        [Required(ErrorMessage = "Rank required")]
        [Display(Name = "Rank")]
        [Range(1, 400)]
        public int Rank { get; set; }

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

        public List<InscriptionModel> Inscriptions { get; set; }
    }

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

    public class InscWeaponTypeModel
    {
        public string PageId { get; set; }
        public string InscId { get; set; }
        public string TypeExtension { get; set; }

        public DataModels.Items.ItemType? Type { get; set; }
    }
}