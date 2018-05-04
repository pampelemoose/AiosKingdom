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
        private static DispatchDbContext _context = new DispatchDbContext();

        public static DataModels.GameServerToken Get(Guid token)
        {
            return _context.Tokens.FirstOrDefault(t => t.Token.Equals(token));
        }

        public static DataModels.GameServerToken Create(Guid userId)
        {
            var token = new DataModels.GameServerToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _context.Tokens.Add(token);
            try
            {
                _context.SaveChanges();
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

        public static bool Remove(DataModels.GameServerToken token)
        {
            _context.Tokens.Remove(token);
            try
            {
                _context.SaveChanges();
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
