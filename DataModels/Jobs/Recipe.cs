using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Jobs
{
    public enum JobTechnique
    {
        Shatter,
        Sand,
        Shape,

        Chop,
        Cut,

        Carve,
        Write,

        Fold,
        Meld,
        Bend,

        Melt,
        Cool,

        Boil,
        Blow,
        Dip,
        Dry,

        Assemble,
        Mix
    }

    public static class RecipeQualityResults
    {
        public class RecipeResult
        {
            public double Common { get; set; }
            public double Uncommon { get; set; }
            public double Rare { get; set; }
            public double Epic { get; set; }
            public double Legendary { get; set; }
        }

        /**
         * This defines the chances to craft the item for each JobRank for each Quality
         * First we gather the recipe ingredients quality score. It set the maximum quality
         * craftable. Then, starting from the highest quality possible, we throw a rand one by one,
         * the first successful throw define the quality. (For now let's test that way.)
         **/
        public static Dictionary<JobRank, RecipeResult> Results = new Dictionary<JobRank, RecipeResult>
        {
            {
                JobRank.Apprentice,
                new RecipeResult
                {
                    Common = 0.8f,
                    Uncommon = 0.01f,
                    Rare = 0.01f,
                    Epic = 0.005f,
                    Legendary = 0.00005f
                }
            },
            {
                JobRank.Practitioner,
                new RecipeResult
                {
                    Common = 0.3f,
                    Uncommon = 0.4f,
                    Rare = 0.04f,
                    Epic = 0.01f,
                    Legendary = 0.0001f
                }
            },
            {
                JobRank.Master,
                new RecipeResult
                {
                    Common = 0.2f,
                    Uncommon = 0.3f,
                    Rare = 0.4f,
                    Epic = 0.04f,
                    Legendary = 0.005f
                }
            },
            {
                JobRank.GrandMaster,
                new RecipeResult
                {
                    Common = 0.1f,
                    Uncommon = 0.2f,
                    Rare = 0.4f,
                    Epic = 0.2f,
                    Legendary = 0.03f
                }
            },
            {
                JobRank.Legend,
                new RecipeResult
                {
                    Common = 0.05f,
                    Uncommon = 0.1f,
                    Rare = 0.3f,
                    Epic = 0.4f,
                    Legendary = 0.08f
                }
            }
        };

        /**
         * This defines the chances to craft the item for each JobRank for each Quality
         * First we gather the recipe ingredients quality score. It set the maximum quality
         * craftable. Then, starting from the highest quality possible, we throw a rand one by one,
         * the first successful throw define the quality. (For now let's test that way.)
         **/
        public static Dictionary<JobRank, RecipeResult> ItemRatios = new Dictionary<JobRank, RecipeResult>
        {
            {
                JobRank.Apprentice,
                new RecipeResult
                {
                    Common = 0.9f,
                    Uncommon = 1.0f,
                    Rare = 1.1f,
                    Epic = 1.2f,
                    Legendary = 1.5f
                }
            },
            {
                JobRank.Practitioner,
                new RecipeResult
                {
                    Common = 1.3f,
                    Uncommon = 1.1f,
                    Rare = 1.2f,
                    Epic = 1.3f,
                    Legendary = 1.5f
                }
            },
            {
                JobRank.Master,
                new RecipeResult
                {
                    Common = 1.4f,
                    Uncommon = 1.2f,
                    Rare = 1.1f,
                    Epic = 1.4f,
                    Legendary = 1.6f
                }
            },
            {
                JobRank.GrandMaster,
                new RecipeResult
                {
                    Common = 2.0f,
                    Uncommon = 1.7f,
                    Rare = 1.4f,
                    Epic = 1.2f,
                    Legendary = 1.7f
                }
            },
            {
                JobRank.Legend,
                new RecipeResult
                {
                    Common = 2.5f,
                    Uncommon = 2.0f,
                    Rare = 1.7f,
                    Epic = 1.5f,
                    Legendary = 1.9f
                }
            }
        };
    }

    public class Recipe : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public JobType JobType { get; set; }

        public int Price { get; set; }

        public JobTechnique Technique { get; set; }

        public List<Combinaison> Combinaisons { get; set; }

        public Guid? CommonCraftedItemId { get; set; }
        public Guid? UncommonCraftedItemId { get; set; }
        public Guid? RareCraftedItemId { get; set; }
        public Guid? EpicCraftedItemId { get; set; }
        public Guid? LegendaryCraftedItemId { get; set; }

        public bool IsShop { get; set; }

        public Guid? CraftedItemId(Items.ItemQuality quality)
        {
            switch (quality)
            {
                case Items.ItemQuality.Common:
                    return CommonCraftedItemId;
                case Items.ItemQuality.Uncommon:
                    return UncommonCraftedItemId;
                case Items.ItemQuality.Rare:
                    return RareCraftedItemId;
                case Items.ItemQuality.Epic:
                    return EpicCraftedItemId;
                case Items.ItemQuality.Legendary:
                    return LegendaryCraftedItemId;
            }

            return null;
        }
    }
}
