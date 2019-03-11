using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website.Authentication;

namespace Website.Controllers
{
    [CustomAuthorize(Roles = "JobCreator")]
    public class RecipesController : AKBaseController
    {
        public ActionResult Index(Models.Filters.RecipeFilter filter)
        {
            var recipes = DataRepositories.RecipeRepository.GetAll();

            filter.Recipes = filter.FilterList(recipes);

            return View(filter);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var recipe = new Models.RecipeModel();

            recipe.LegendaryCraftedItemId = null;

            recipe.Combinaisons = new List<DataModels.Jobs.Combinaison>();

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.RecipeModel recipeModel)
        {
            if (recipeModel.Combinaisons == null)
            {
                recipeModel.Combinaisons = new List<DataModels.Jobs.Combinaison>();
            }

            if (ModelState.IsValid)
            {
                if (recipeModel.JobType == DataModels.Jobs.JobType.None)
                {
                    Alert(AlertMessage.AlertType.Danger, $"You need to select a valid job.");
                    return View(recipeModel);
                }

                if (recipeModel.Combinaisons.Count == 0)
                {
                    Alert(AlertMessage.AlertType.Danger, $"You need at least one Combinaison and one Talent.");
                    return View(recipeModel);
                }

                var recipeId = Guid.NewGuid();
                var recipe = new DataModels.Jobs.Recipe
                {
                    Id = Guid.NewGuid(),
                    VersionId = recipeModel.SelectedVersion,
                    Vid = recipeId,
                    Name = recipeModel.Name,
                    JobType = recipeModel.JobType,
                    Price = recipeModel.Price,
                    Technique = recipeModel.Technique,
                    Combinaisons = new List<DataModels.Jobs.Combinaison>(),
                    CommonCraftedItemId = recipeModel.CommonCraftedItemId,
                    UncommonCraftedItemId = recipeModel.UncommonCraftedItemId,
                    RareCraftedItemId = recipeModel.RareCraftedItemId,
                    EpicCraftedItemId = recipeModel.EpicCraftedItemId,
                    LegendaryCraftedItemId = recipeModel.LegendaryCraftedItemId,
                    IsShop = recipeModel.IsShop
                };

                foreach (var combModel in recipeModel.Combinaisons)
                {
                    var comb = new DataModels.Jobs.Combinaison
                    {
                        Id = Guid.NewGuid(),
                        CommonItemId = combModel.CommonItemId,
                        UncommonItemId = combModel.UncommonItemId,
                        RareItemId = combModel.RareItemId,
                        EpicItemId = combModel.EpicItemId,
                        LegendaryItemId = combModel.LegendaryItemId,
                        PercentagePerItem = combModel.PercentagePerItem,
                        MinQuantity = combModel.MinQuantity,
                        MaxQuantity = combModel.MaxQuantity
                    };

                    recipe.Combinaisons.Add(comb);
                }

                if (DataRepositories.RecipeRepository.Create(recipe))
                {
                    return RedirectToAction("Index");
                }
            }

            return View(recipeModel);
        }

        [HttpGet]
        public ActionResult AddCombinaisonPartial()
        {
            var comb = new DataModels.Jobs.Combinaison();
            comb.Id = Guid.NewGuid();

            return PartialView("CombinaisonPartial", comb);
        }
    }
}