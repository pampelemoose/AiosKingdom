using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Monsters
{
    public class Phase
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid MonsterId { get; set; }

        [Required]
        [Range(1, 10000)]
        public double StaminaPerLevel { get; set; }

        [Required]
        [Range(1, 10000)]
        public double EnergyPerLevel { get; set; }

        [Required]
        [Range(1, 10000)]
        public double StrengthPerLevel { get; set; }

        [Required]
        [Range(1, 10000)]
        public double AgilityPerLevel { get; set; }

        [Required]
        [Range(1, 10000)]
        public double IntelligencePerLevel { get; set; }

        [Required]
        [Range(1, 10000)]
        public double WisdomPerLevel { get; set; }

        [Required]
        public List<Skills.Page> Skills { get; set; }
    }
}
