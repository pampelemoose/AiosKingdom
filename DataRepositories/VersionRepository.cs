using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class VersionRepository
    {
        public static List<DataModels.Version> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Versions.ToList();
            }
        }
    }
}
