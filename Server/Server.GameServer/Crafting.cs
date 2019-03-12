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
            double[] recipeToken = { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

            foreach (var component in components)
            {
                var item = DataManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(component.ItemId));
                var combinaison = recipe.Combinaisons.FirstOrDefault(c => c.ItemId(item.Quality).Equals(item.Id));
                int itemQuality = (int)item.Quality;

                if (itemQuality >= 4)
                {

                    recipeToken[4] += (component.Quantity * itemRatios.Legendary * combinaison.PercentagePerItem);
                    recipeToken[3] += (component.Quantity * itemRatios.Epic * combinaison.PercentagePerItem);
                    recipeToken[2] += (component.Quantity * itemRatios.Rare * combinaison.PercentagePerItem);
                    recipeToken[1] += (component.Quantity * itemRatios.Uncommon * combinaison.PercentagePerItem);
                    recipeToken[0] += (component.Quantity * itemRatios.Common * combinaison.PercentagePerItem);
                }
                if (itemQuality >= 3)
                {
                    recipeToken[3] += (component.Quantity * itemRatios.Epic * combinaison.PercentagePerItem);
                    recipeToken[2] += (component.Quantity * itemRatios.Rare * combinaison.PercentagePerItem);
                    recipeToken[1] += (component.Quantity * itemRatios.Uncommon * combinaison.PercentagePerItem);
                    recipeToken[0] += (component.Quantity * itemRatios.Common * combinaison.PercentagePerItem);
                }
                if (itemQuality >= 2)
                {
                    recipeToken[2] += (component.Quantity * itemRatios.Rare * combinaison.PercentagePerItem);
                    recipeToken[1] += (component.Quantity * itemRatios.Uncommon * combinaison.PercentagePerItem);
                    recipeToken[0] += (component.Quantity * itemRatios.Common * combinaison.PercentagePerItem);
                }
                if (itemQuality >= 1)
                {
                    recipeToken[1] += (component.Quantity * itemRatios.Uncommon * combinaison.PercentagePerItem);
                    recipeToken[0] += (component.Quantity * itemRatios.Common * combinaison.PercentagePerItem);
                }
                if (itemQuality >= 0)
                {
                    recipeToken[0] += (component.Quantity * itemRatios.Common * combinaison.PercentagePerItem);
                }
            }

            double commonFinalTicket = recipeToken[0] * recipeQuality.Common;
            double uncommonFinalTicket = recipeToken[1] * recipeQuality.Uncommon;
            double rareFinalTicket = recipeToken[2] * recipeQuality.Rare;
            double epicFinalTicket = recipeToken[3] * recipeQuality.Epic;
            double LegendaryFinalTicket = recipeToken[4] * recipeQuality.Legendary;

            var rand = new Random();
            var commonChances = rand.Next(0, (int)Math.Round(commonFinalTicket * 100)) / 100.0f;
            var uncommonChances = rand.Next(0, (int)Math.Round(uncommonFinalTicket * 100)) / 100.0f;
            var rareChances = rand.Next(0, (int)Math.Round(rareFinalTicket * 100)) / 100.0f;
            var epicChances = rand.Next(0, (int)Math.Round(epicFinalTicket * 100)) / 100.0f;
            var legendaryChances = rand.Next(0, (int)Math.Round(LegendaryFinalTicket * 100)) / 100.0f;

            Console.WriteLine($"Crafting chances C[{commonChances}],UC[{uncommonChances}],R[{rareChances}], E[{epicChances}], L[{legendaryChances}] .");

            Guid? itemId = null;
            if (commonChances > uncommonChances
                && commonChances > rareChances
                && commonChances > epicChances
                && commonChances > legendaryChances)
            {
                itemId = recipe.CraftedItemId(Network.Items.ItemQuality.Common);
            }
            else if (uncommonChances > rareChances
                && uncommonChances > epicChances
                && uncommonChances > legendaryChances)
            {
                itemId = recipe.CraftedItemId(Network.Items.ItemQuality.Uncommon);
            }
            else if (rareChances > epicChances
                && rareChances > legendaryChances)
            {
                itemId = recipe.CraftedItemId(Network.Items.ItemQuality.Rare);
            }
            else if (epicChances > legendaryChances)
            {
                itemId = recipe.CraftedItemId(Network.Items.ItemQuality.Epic);
            }
            else
            {
                itemId = recipe.CraftedItemId(Network.Items.ItemQuality.Legendary);
            }

            return DataManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(itemId));
        }
    }
}
