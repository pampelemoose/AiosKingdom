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

    public class Town
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VersionId { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
        public bool Online { get; set; }

        public Guid KingdomId { get; set; }

        public string Name { get; set; }
        public ServerDifficulty Difficulty { get; set; }
        public int SlotLimit { get; set; }
        public int SlotAvailable { get; set; }

        public int BaseExperience { get; set; }
        public float ExperiencePerLevelRatio { get; set; }

        public int SpiritsPerLevelUp { get; set; }
        public int EmbersPerLevelUp { get; set; }

        public int BaseHealth { get; set; }
        public float HealthPerLevelRatio { get; set; }
        public float HealthPerStaminaRatio { get; set; }

        public int BaseMana { get; set; }
        public float ManaPerLevelRatio { get; set; }
        public float ManaPerEnergyRatio { get; set; }

        public Guid DefaultBagId { get; set; }

        public int PractitionerJobPoints { get; set; }
        public int PractitionerRecipeCount { get; set; }
        public int MasterJobPoints { get; set; }
        public int MasterRecipeCount { get; set; }
        public int GrandMasterJobPoints { get; set; }
        public int GrandMasterRecipeCount { get; set; }
        public int LegendJobPoints { get; set; }
        public int LegendRecipeCount { get; set; }
    }
}
