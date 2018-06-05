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
        [Range(0.001, 100, ErrorMessage = "ExperiencePerLevelRatio should be higher than 0")]
        public double ExperiencePerLevelRatio { get; set; }

        [Display(Name = "Loots")]
        public List<LootModel> Loots { get; set; }

        [Display(Name = "Phases")]
        public List<PhaseModel> Phases { get; set; }
    }
}