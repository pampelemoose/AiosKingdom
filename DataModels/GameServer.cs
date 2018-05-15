using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels
{
    public enum ServerDifficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2,
        Legendary = 3
    }

    public class GameServer
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VersionId { get; set; }

        [Required]
        public string Host { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public bool Online { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public ServerDifficulty Difficulty { get; set; }
        [Required]
        public int SlotLimit { get; set; }
        [Required]
        public int SlotAvailable { get; set; }

        [Required]
        public int BaseExperience { get; set; }
        [Required]
        public float ExperiencePerLevelRatio { get; set; }

        [Required]
        public int SpiritsPerLevelUp { get; set; }
        [Required]
        public int EmbersPerLevelUp { get; set; }

        [Required]
        public int BaseHealth { get; set; }
        [Required]
        public float HealthPerLevelRatio { get; set; }
        [Required]
        public float HealthPerStaminaRatio { get; set; }

        [Required]
        public int BaseMana { get; set; }
        [Required]
        public float ManaPerLevelRatio { get; set; }
        [Required]
        public float ManaPerEnergyRatio { get; set; }
    }
}
