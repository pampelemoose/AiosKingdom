using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class JobRepository
    {
        public static List<DataModels.Jobs.Job> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Jobs
                    .Include(a => a.Recipes)
                    .ToList();
            }
        }
    }
}
