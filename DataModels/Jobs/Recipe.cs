using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Jobs
{
    public enum JobTechnique
    {
        Shatter,
        Sand,

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
                    Common = 80.0f,
                    Uncommon = 10.0f,
                    Rare = 1.0f,
                    Epic = 0.5f,
                    Legendary = 0.05f
                }
            },
            {
                JobRank.Practitioner,
                new RecipeResult
                {
                    Common = 30.0f,
                    Uncommon = 40.0f,
                    Rare = 4.0f,
                    Epic = 1.0f,
                    Legendary = 0.1f
                }
            },
            {
                JobRank.Master,
                new RecipeResult
                {
                    Common = 20.0f,
                    Uncommon = 30.0f,
                    Rare = 40.0f,
                    Epic = 4.0f,
                    Legendary = 0.5f
                }
            },
            {
                JobRank.GrandMaster,
                new RecipeResult
                {
                    Common = 10.0f,
                    Uncommon = 20.0f,
                    Rare = 40.0f,
                    Epic = 20.0f,
                    Legendary = 3.0f
                }
            },
            {
                JobRank.Legend,
                new RecipeResult
                {
                    Common = 5.0f,
                    Uncommon = 10.0f,
                    Rare = 30.0f,
                    Epic = 40.0f,
                    Legendary = 8.0f
                }
            }
        };
    }

    public class Recipe : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public JobType JobType { get; set; }

        public int Price { get; set; }

        public JobTechnique Technique { get; set; }

        public List<Combinaison> Combinaisons { get; set; }

        public Guid CraftedItemId { get; set; }
    }
}
