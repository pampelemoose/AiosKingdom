using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public static class Crafting
    {
        public static Network.Items.Item CraftItem(ref Network.Job job, Guid recipeId, Network.JobTechnique technique, List<Network.CraftingComponent> components)
        {
            var localJob = job;
            var recipe = DataManager.Instance.Recipes.FirstOrDefault(r => r.Id.Equals(recipeId) && r.JobType == localJob.Type && r.Technique == technique);

            if (recipe != null)
            {
                var componentIds = components.Select(c => c.ItemId).ToList();

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
            double legendaryFinalTicket = recipeToken[4] * recipeQuality.Legendary;

            Log.Instance.Write(Log.Type.Job, Log.Level.Infos, $"Crafting Tickets: common[{commonFinalTicket}], uncommon[{uncommonFinalTicket}], rare[{rareFinalTicket}], epic[{epicFinalTicket}], legendary[{legendaryFinalTicket}]");

            var rand = new Random();
            var randTokenRaw = rand.Next(0, 100000000);
            double randToken = randTokenRaw / 1000000.0f;

            Log.Instance.Write(Log.Type.Job, Log.Level.Infos, $"Rand token [{randToken}]");

            Guid? itemId = null;
            if (commonFinalTicket >= randToken)
            {
                itemId = recipe.CraftedItemId(Network.Items.ItemQuality.Common);
            }
            if (uncommonFinalTicket >= randToken)
            {
                var overItemId = recipe.CraftedItemId(Network.Items.ItemQuality.Uncommon);

                if (overItemId != null)
                {
                    itemId = overItemId;
                }
            }
            if (rareFinalTicket >= randToken)
            {
                var overItemId = recipe.CraftedItemId(Network.Items.ItemQuality.Rare);

                if (overItemId != null)
                {
                    itemId = overItemId;
                }
            }
            if (epicFinalTicket >= randToken)
            {
                var overItemId = recipe.CraftedItemId(Network.Items.ItemQuality.Epic);

                if (overItemId != null)
                {
                    itemId = overItemId;
                }
            }
            if (legendaryFinalTicket >= randToken)
            {
                var overItemId = recipe.CraftedItemId(Network.Items.ItemQuality.Legendary);

                if (overItemId != null)
                {
                    itemId = overItemId;
                }
            }

            Log.Instance.Write(Log.Type.Job, Log.Level.Infos, $"Crafted id: [{itemId}]");

            return DataManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(itemId));
        }
    }
}
