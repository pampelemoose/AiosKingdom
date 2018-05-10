using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class TokenRepository
    {
        public static DataModels.GameServerToken Get(Guid token)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Tokens.FirstOrDefault(t => t.Token.Equals(token));
            }
        }

        public static DataModels.GameServerToken Create(Guid userId)
        {
            using (var context = new AiosKingdomContext())
            {
                var token = new DataModels.GameServerToken
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

        public static bool Remove(DataModels.GameServerToken token)
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
