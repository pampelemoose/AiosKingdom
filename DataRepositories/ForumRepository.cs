using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class ForumRepository
    {
        public static List<DataModels.Website.Category> GetAllCategories()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Categories
                    .Include(s => s.Threads)
                    .Include(s => s.Threads.Select(t => t.Comments))
                    .ToList();
            }
        }

        public static DataModels.Website.Category GetCategoryById(int id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Categories
                    .Include(s => s.Threads)
                    .Include(s => s.Threads.Select(t => t.Comments))
                    .FirstOrDefault(s => s.Id == id);
            }
        }

        public static DataModels.Website.Thread GetThreadById(int id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Threads
                    .Include(s => s.Category)
                    .Include(s => s.Comments)
                    .FirstOrDefault(s => s.Id == id);
            }
        }

        public static bool CreateCategory(DataModels.Website.Category category)
        {
            using (var context = new AiosKingdomContext())
            {
                context.Categories.Add(category);

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

        public static bool CreateThread(DataModels.Website.Thread thread, int categoryId)
        {
            using (var context = new AiosKingdomContext())
            {
                thread.Category = context.Categories.FirstOrDefault(t => t.Id == categoryId);

                context.Threads.Add(thread);

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

        public static bool CreateComment(DataModels.Website.Comment comment, int threadId)
        {
            using (var context = new AiosKingdomContext())
            {
                comment.Thread = context.Threads.FirstOrDefault(t => t.Id == threadId);

                context.Comments.Add(comment);

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

        public static bool DeleteThread(int threadId)
        {
            using (var context = new AiosKingdomContext())
            {
                var exists = context.Threads.FirstOrDefault(t => t.Id == threadId);

                if (exists == null) return false;
                
                context.Threads.Remove(exists);

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

        public static bool DeleteComment(int commentId)
        {
            using (var context = new AiosKingdomContext())
            {
                var exists = context.Comments.FirstOrDefault(t => t.Id == commentId);

                if (exists == null) return false;

                context.Comments.Remove(exists);

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
