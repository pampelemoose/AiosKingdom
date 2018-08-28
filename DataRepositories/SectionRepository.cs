using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class SectionRepository
    {
        public static List<DataModels.Website.Section> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Sections
                    .Include(s => s.Contents)
                    .ToList();
            }
        }

        public static DataModels.Website.Section GetById(int id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Sections
                    .Include(s => s.Contents)
                    .FirstOrDefault(s => s.Id.Equals(id));
            }
        }

        public static List<DataModels.Website.Section> GetForPage(string action, string controller, bool before)
        {
            using (var context = new AiosKingdomContext())
            {
                var section = context.Sections
                    .Include(s => s.Contents)
                    .Where(s => s.Action.Equals(action) && s.Controller.Equals(controller) && s.Before == before)
                    .OrderBy(s => s.Order)
                    .ToList();

                var sections = new List<DataModels.Website.Section>();
                foreach (var sec in section)
                {
                    sec.Contents.RemoveAll(c => c.Active == false);

                    if (sec.Contents.Count > 0)
                    {
                        sec.Contents = sec.Contents.OrderBy(c => c.Order).ToList();
                        sections.Add(sec);
                    }
                }

                if (sections.Count == 0)
                {
                    return null;
                }

                return sections;
            }
        }

        public static bool Create(DataModels.Website.Section section)
        {
            using (var context = new AiosKingdomContext())
            {
                context.Sections.Add(section);

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

        public static bool Update(DataModels.Website.Section section)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Sections
                    .Include(p => p.Contents)
                    .FirstOrDefault(u => u.Id.Equals(section.Id));

                if (online == null)
                    return false;

                online.Controller = section.Controller;
                online.Action = section.Action;
                online.Order = section.Order;
                online.Before = section.Before;
                online.Type = section.Type;

                // CONTENTS
                var oldContent = online.Contents;
                online.Contents = new List<DataModels.Website.Banner>();
                foreach (var content in section.Contents)
                {
                    var exists = oldContent.FirstOrDefault(c => c.Id.Equals(content.Id));

                    if (exists == null)
                    {
                        online.Contents.Add(content);
                    }
                    else
                    {
                        exists.SectionId = section.Id;
                        exists.Image = content.Image;
                        exists.BackgroundColor = content.BackgroundColor;
                        exists.Content = content.Content;
                        exists.Active = content.Active;
                        exists.ImageSide = content.ImageSide;
                        exists.TextColor = content.TextColor;
                        exists.Order = content.Order;

                        online.Contents.Add(content);
                        oldContent.Remove(oldContent.FirstOrDefault(o => o.Id.Equals(content.Id)));
                    }
                }

                foreach (var toDel in oldContent)
                {
                    context.Contents.Remove(toDel);
                }

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
