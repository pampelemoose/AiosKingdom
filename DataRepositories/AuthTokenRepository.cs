using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class AuthTokenRepository
    {
        public static DataModels.AuthToken Get(Guid token)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Tokens.FirstOrDefault(t => t.Token.Equals(token));
            }
        }

        public static DataModels.AuthToken Create(Guid userId)
        {
            using (var context = new AiosKingdomContext())
            {
                var token = new DataModels.AuthToken
                {
                    Id = Guid.NewGuid(),
                    Token = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };

                context.Tokens.Add(token);
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
                    return null;
                }
                return token;
            }
        }

        public static bool Remove(DataModels.AuthToken token)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Tokens.FirstOrDefault(t => t.Id.Equals(token.Id));
                if (online == null) return false;

                context.Tokens.Remove(online);
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
