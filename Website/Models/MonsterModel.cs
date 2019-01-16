using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class MonsterModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

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

        [Required(ErrorMessage = "BaseHealth required")]
        [Display(Name = "BaseHealth")]
        [Range(1, 10000000, ErrorMessage = "BaseHealth should be higher than 0")]
        public int BaseHealth { get; set; }

        [Required(ErrorMessage = "HealthPerLevel required")]
        [Display(Name = "HealthPerLevel")]
        [Range(1, 10000, ErrorMessage = "HealthPerLevel should be higher than 0")]
        public double HealthPerLevel { get; set; }

        [Required(ErrorMessage = "BaseExperience required")]
        [Display(Name = "BaseExperience")]
        [Range(1, 1000000, ErrorMessage = "BaseExperience should be higher than 0")]
        public int BaseExperience { get; set; }

        [Required(ErrorMessage = "ExperiencePerLevelRatio required")]
        [Display(Name = "ExperiencePerLevelRatio")]
        [Range(1, 10000, ErrorMessage = "ExperiencePerLevelRatio should be higher than 0")]
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

    public class LootModel
    {
        public Guid Id { get; set; }

        [Required]
        [Range(0.00001, 100)]
        public double DropRate { get; set; }

        public Guid SelectedItem { get; set; }
        [Display(Name = "Item")]
        public List<DataModels.Items.Item> Items
        {
            get
            {
                var items = DataRepositories.ItemRepository.GetAll().ToList();

                return items;
            }
        }

        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
    }

    public class PhaseModel
    {
        public class SkillChoice
        {
            public Guid VersionId { get; set; }
            public string Name { get; set; }
            public Guid SkillId { get; set; }
        }

        public Guid Id { get; set; }

        public Guid SelectedVersion { get; set; }

        [Required]
        public Guid SelectedSkill { get; set; }
        [Display(Name = "Skill")]
        public List<SkillChoice> Skills
        {
            get
            {
                var books = new List<SkillChoice>();

                foreach (var book in DataRepositories.BookRepository.GetAll())
                {
                    books.Add(new SkillChoice
                    {
                        VersionId = book.VersionId,
                        Name = book.Name,
                        SkillId = book.Vid,
                    });
                }

                return books;
            }
        }
    }
}