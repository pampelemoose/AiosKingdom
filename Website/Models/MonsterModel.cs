using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class MonsterModel
    {
        [Required]
        public Guid SelectedVersion { get; set; }
        [Display(Name = "Version")]
        public List<DataModels.Version> VersionList { get; set; }

        [Display(Name = "Types")]
        public List<DataModels.Monsters.MonsterType> Types { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Story required"), MinLength(4), MaxLength(2000)]
        [Display(Name = "Story")]
        public string Story { get; set; }

        private string _image = "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";
        [Display(Name = "Image")]
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        [Required(ErrorMessage = "HealthPerLevel required")]
        [Display(Name = "HealthPerLevel")]
        [Range(1, 10000, ErrorMessage = "HealthPerLevel should be higher than 0")]
        public double HealthPerLevel { get; set; }

        [Required(ErrorMessage = "BaseExperience required")]
        [Display(Name = "BaseExperience")]
        public int BaseExperience { get; set; }

        [Required(ErrorMessage = "ExperiencePerLevelRatio required")]
        [Display(Name = "ExperiencePerLevelRatio")]
        [Range(0, 10000, ErrorMessage = "ExperiencePerLevelRatio should be higher than 0")]
        public double ExperiencePerLevelRatio { get; set; }

        [Required(ErrorMessage = "StaminaPerLevel required")]
        [Display(Name = "StaminaPerLevel")]
        [Range(0, 10000, ErrorMessage = "StaminaPerLevel should be higher than 0")]
        public double StaminaPerLevel { get; set; }

        [Required(ErrorMessage = "EnergyPerLevel required")]
        [Display(Name = "EnergyPerLevel")]
        [Range(0, 10000, ErrorMessage = "EnergyPerLevel should be higher than 0")]
        public double EnergyPerLevel { get; set; }

        [Required(ErrorMessage = "StrengthPerLevel required")]
        [Display(Name = "StrengthPerLevel")]
        [Range(0, 10000, ErrorMessage = "StrengthPerLevel should be higher than 0")]
        public double StrengthPerLevel { get; set; }

        [Required(ErrorMessage = "AgilityPerLevel required")]
        [Display(Name = "AgilityPerLevel")]
        [Range(0, 10000, ErrorMessage = "AgilityPerLevel should be higher than 0")]
        public double AgilityPerLevel { get; set; }

        [Required(ErrorMessage = "IntelligencePerLevel required")]
        [Display(Name = "IntelligencePerLevel")]
        [Range(0, 10000, ErrorMessage = "IntelligencePerLevel should be higher than 0")]
        public double IntelligencePerLevel { get; set; }

        [Required(ErrorMessage = "WisdomPerLevel required")]
        [Display(Name = "WisdomPerLevel")]
        [Range(0, 10000, ErrorMessage = "WisdomPerLevel should be higher than 0")]
        public double WisdomPerLevel { get; set; }

        [Display(Name = "Loots")]
        public List<LootModel> Loots { get; set; }

        [Display(Name = "Phases")]
        public List<PhaseModel> Phases { get; set; }
    }
}