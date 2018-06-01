using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class SectionRepository
    {
        public static DataModels.Website.Section GetForPage(string action, string controller, bool before)
        {
            using (var context = new AiosKingdomContext())
            {
                var section = context.Sections.Include(s => s.Contents)
                    .FirstOrDefault(s => s.Action.Equals(action) && s.Controller.Equals(controller) && s.Before == before);

                if (section != null)
                {
                    section.Contents.RemoveAll(c => c.Active == false);

                    if (section.Contents.Count == 0)
                        return null;
                }

                return section;
            }
        }
    }
}
