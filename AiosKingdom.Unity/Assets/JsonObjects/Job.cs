using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonObjects
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
        public Guid Id { get; set; }

        public JobType Type { get; set; }

        public int Points { get; set; }

        public JobRank Rank { get; set; }

        public List<RecipeUnlocked> Recipes { get; set; }
    }

    public class RecipeUnlocked
    {
        public Guid Id { get; set; }

        public Guid SoulId { get; set; }
        public Guid RecipeId { get; set; }

        public DateTime? UnlockedAt { get; set; }

        public bool IsNew { get; set; }
    }

    public class CraftingComponent
    {
        public Guid ItemId { get; set; }
        public Guid InventoryId { get; set; }
        public int Quantity { get; set; }
    }
}
