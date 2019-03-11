using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class RecipeRepository
    {
        public static List<DataModels.Jobs.Recipe> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Recipes
                    .Include(a => a.Combinaisons)
                    .ToList();
            }
        }

        public static List<DataModels.Jobs.Recipe> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Recipes
                    .Where(r => r.VersionId.Equals(versionId))
                    .Include(a => a.Combinaisons)
                    .ToList();
            }
        }

        public static bool Create(DataModels.Jobs.Recipe recipe)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Recipes.FirstOrDefault(u => u.Name.Equals(recipe.Name)) != null)
                    return false;

                if (recipe.Id.Equals(Guid.Empty))
                    return false;

                context.Recipes.Add(recipe);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var error in e.EntityValidationErrors)
                    {
                        foreach (var mess in error.ValidationErrors)
                        {
                            Console.WriteLine(mess.ErrorMessage);
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
