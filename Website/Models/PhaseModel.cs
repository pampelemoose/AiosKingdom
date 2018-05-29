using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class PhaseModel
    {
        [Required]
        [Range(0, 10000)]
        public double StaminaPerLevel { get; set; }

        [Required]
        [Range(0, 10000)]
        public double EnergyPerLevel { get; set; }

        [Required]
        [Range(0, 10000)]
        public double StrengthPerLevel { get; set; }

        [Required]
        [Range(0, 10000)]
        public double AgilityPerLevel { get; set; }

        [Required]
        [Range(0, 10000)]
        public double IntelligencePerLevel { get; set; }

        [Required]
        [Range(0, 10000)]
        public double WisdomPerLevel { get; set; }

        [Required]
        public Guid SelectedSkill { get; set; }
        [Display(Name = "Item")]
        public List<DataModels.Skills.Page> Skills
        {
            get
            {
                var pages = new List<DataModels.Skills.Page>();

                foreach (var book in DataRepositories.BookRepository.GetAll())
                {
                    pages.AddRange(book.Pages);
                }

                return pages;
            }
        }
    }
}