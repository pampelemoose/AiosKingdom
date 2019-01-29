using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Jobs
{
    public enum JobType
    {
        None,

        Mining,
        Herborism,
        
        Alchemistry,
        ScrollWriting,

        RuneSmithing,
        Enchanting,

        WeaponSmithing,
        ArmorSmithing
    }

    public enum JobRank
    {
        Apprentice,
        Practitioner,
        Master,
        GrandMaster,
        Legend
    }

    public class Job
    {
        [Key]
        public Guid Id { get; set; }

        public JobType Type { get; set; }

        public int Points { get; set; }

        public JobRank Rank { get; set; }
        public int Experience { get; set; }

        public List<RecipeUnlocked> Recipes { get; set; }
        public List<DiscoveryUnlocked> Discoveries { get; set; }
    }
}
