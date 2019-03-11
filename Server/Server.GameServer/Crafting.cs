using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public static class Crafting
    {
        public static Network.Items.Item CraftItem(ref Network.Job job, Network.JobTechnique technique, List<Network.CraftingComponent> components)
        {
            var localJob = job;
            var jobRecipes = DataManager.Instance.Recipes.Where(r => r.JobType == localJob.Type && r.Technique == technique);

            if (jobRecipes != null)
            {
                var componentIds = components.Select(c => c.ItemId).ToList();
                foreach (var recipe in jobRecipes)
                {
                    var isRecipe = true;
                    foreach (var combinaison in recipe.Combinaisons)
                    {
                        int componentCombinaisonCount = 0;
                        foreach (Network.Items.ItemQuality quality in Enum.GetValues(typeof(Network.Items.ItemQuality)))
                        {
                            var component = components.FirstOrDefault(c => c.ItemId.Equals(combinaison.ItemId(quality)));
                            if (component != null)
                            {
                                componentCombinaisonCount += component.Quantity;
                            }
                        }

                        if (combinaison.MinQuantity > componentCombinaisonCount || combinaison.MaxQuantity < componentCombinaisonCount)
                        {
                            isRecipe = false;
                            break;
                        }
                    }

                    //foreach (var component in components)
                    //{
                    //    var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId.Equals(component.ItemId));

                    //    if (combinaison.MinQuantity > component.Quantity || combinaison.MaxQuantity < component.Quantity)
                    //    {
                    //        isRecipe = false;
                    //        break;
                    //    }
                    //}

                    if (isRecipe)
                    {
                        if (!localJob.Recipes.Any(r => r.RecipeId.Equals(recipe.Id)))
                        {
                            job.Recipes.Add(new Network.RecipeUnlocked
                            {
                                Id = Guid.NewGuid(),
                                RecipeId = recipe.Id,
                                IsNew = true,
                                UnlockedAt = DateTime.Now
                            });
                        }

                        var item = _craftItem(localJob, recipe, components);

                        return item;
                    }
                }
            }

            return null;
        }

        private static Network.Items.Item _craftItem(Network.Job job, Network.Recipe recipe, List<Network.CraftingComponent> components)
        {
            var rank = DataManager.ConvertBackJobRank(job.Rank);
            var recipeQuality = DataModels.Jobs.RecipeQualityResults.Results[rank];
            var itemRatios = DataModels.Jobs.RecipeQualityResults.ItemRatios[rank];

            var jobToken = ((int)job.Rank) + 1;
            double recipeToken = 0.0f;

            foreach (var component in components)
            {
                var item = DataManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(component.ItemId));

                switch (item.Quality)
                {
                    case Network.Items.ItemQuality.Common:
                        {

                            var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId(Network.Items.ItemQuality.Common).Equals(item.Id));
                            recipeToken += (component.Quantity * itemRatios.Common * combinaison.PercentagePerItem);
                        }
                        break;
                    case Network.Items.ItemQuality.Uncommon:
                        {
                            var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId(Network.Items.ItemQuality.Uncommon).Equals(item.Id));
                            recipeToken += (component.Quantity * itemRatios.Uncommon * combinaison.PercentagePerItem);
                        }
                        break;
                    case Network.Items.ItemQuality.Rare:
                        {
                            var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId(Network.Items.ItemQuality.Rare).Equals(item.Id));
                            recipeToken += (component.Quantity * itemRatios.Rare * combinaison.PercentagePerItem);
                        }
                        break;
                    case Network.Items.ItemQuality.Epic:
                        {
                            var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId(Network.Items.ItemQuality.Epic).Equals(item.Id));
                            recipeToken += (component.Quantity * itemRatios.Epic * combinaison.PercentagePerItem);
                        }
                        break;
                    case Network.Items.ItemQuality.Legendary:
                        {
                            var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId(Network.Items.ItemQuality.Legendary).Equals(item.Id));
                            recipeToken += (component.Quantity * itemRatios.Legendary * combinaison.PercentagePerItem);
                        }
                        break;
                }
            }

            double commonFinalTicket = recipeToken * recipeQuality.Common;
            double uncommonFinalTicket = recipeToken * recipeQuality.Uncommon;
            double rareFinalTicket = recipeToken * recipeQuality.Rare;
            double epicFinalTicket = recipeToken * recipeQuality.Epic;
            double LegendaryFinalTicket = recipeToken * recipeQuality.Legendary;

            var rand = new Random();
            var commonChances = commonFinalTicket * rand.Next(0, 100);
            var uncommonChances = uncommonFinalTicket * rand.Next(0, 100);
            var rareChances = rareFinalTicket * rand.Next(0, 100);
            var epicChances = epicFinalTicket * rand.Next(0, 100);
            var legendaryChances = LegendaryFinalTicket * rand.Next(0, 100);

            return null;
        }

        private static Network.Items.ItemQuality _calculateCraftingQuality(Network.JobRank jobRank, List<Network.CraftingComponent> components)
        {
            var rank = DataManager.ConvertBackJobRank(jobRank);
            var recipeQuality = DataModels.Jobs.RecipeQualityResults.Results[rank];

            foreach (var component in components)
            {
                var item = DataManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(component.ItemId));

                switch (item.Quality)
                {

                }
            }

            return Network.Items.ItemQuality.Common;
        }
    }
}
