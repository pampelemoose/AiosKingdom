using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public class GameDbContextFactory : IDbContextFactory<GameDbContext>
    {
        public GameDbContext Create()
        {
            return new GameDbContext("RealPG_GameServer_1");
        }
    }
}
