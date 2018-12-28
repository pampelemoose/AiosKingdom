using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public class MonsterRepository
    {
        public static List<DataModels.Monsters.Monster> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Monsters
                    .ToList();
            }
        }

        public static List<DataModels.Monsters.Monster> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Monsters
                    .Include(a => a.Phases)
                    .Include(a => a.Loots)
                    .Where(b => b.VersionId.Equals(versionId))
                    .ToList();
            }
        }

        public static DataModels.Monsters.Monster GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Monsters
                    .Include(a => a.Loots)
                    .Include(a => a.Phases)
                    .FirstOrDefault(a => a.Vid.Equals(id));
            }
        }

        public static bool Create(DataModels.Monsters.Monster monster)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Monsters.FirstOrDefault(u => u.Name.Equals(monster.Name)) != null)
                    return false;

                if (monster.Id.Equals(Guid.Empty))
                    return false;

                context.Monsters.Add(monster);
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

        public static bool Update(DataModels.Monsters.Monster monster)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Monsters
                    .Include(a => a.Loots)
                    .Include(a => a.Phases)
                    .FirstOrDefault(u => u.Id.Equals(monster.Id));

                if (online == null)
                    return false;

                online.VersionId = monster.VersionId;
                online.Name = monster.Name;
                online.Description = monster.Description;
                online.Story = monster.Story;
                online.Types = monster.Types;
                online.BaseHealth = monster.BaseHealth;
                online.HealthPerLevel = monster.HealthPerLevel;
                online.BaseExperience = monster.BaseExperience;
                online.ExperiencePerLevelRatio = monster.ExperiencePerLevelRatio;
                online.StaminaPerLevel = monster.StaminaPerLevel;
                online.EnergyPerLevel = monster.EnergyPerLevel;
                online.StrengthPerLevel = monster.StrengthPerLevel;
                online.AgilityPerLevel = monster.AgilityPerLevel;
                online.IntelligencePerLevel = monster.IntelligencePerLevel;
                online.WisdomPerLevel = monster.WisdomPerLevel;

                // LOOTS
                var oldLoots = online.Loots;
                online.Loots = new List<DataModels.Monsters.Loot>();
                foreach (var item in monster.Loots)
                {
                    if (Guid.Empty.Equals(item.Id))
                    {
                        item.Id = Guid.NewGuid();
                        online.Loots.Add(item);
                    }
                    else
                    {
                        var loo = context.Loots.FirstOrDefault(i => i.Id.Equals(item.Id));
                        loo.Quantity = item.Quantity;
                        loo.ItemVid = item.ItemVid;
                        loo.DropRate = item.DropRate;
                        online.Loots.Add(loo);
                        oldLoots.Remove(oldLoots.FirstOrDefault(o => o.Id.Equals(item.Id)));
                    }
                }

                foreach (var toDel in oldLoots)
                {
                    context.Loots.Remove(toDel);
                }

                // PHASES
                var oldPhases = online.Phases;
                online.Phases = new List<DataModels.Monsters.Phase>();
                foreach (var phase in monster.Phases)
                {
                    if (Guid.Empty.Equals(phase.Id))
                    {
                        phase.Id = Guid.NewGuid();
                        online.Phases.Add(phase);
                    }
                    else
                    {
                        var pha = context.Phases.FirstOrDefault(i => i.Id.Equals(phase.Id));
                        pha.BookVid = phase.BookVid;
                        online.Phases.Add(pha);
                        oldPhases.Remove(oldPhases.FirstOrDefault(o => o.Id.Equals(phase.Id)));
                    }
                }

                foreach (var toDel in oldPhases)
                {
                    context.Phases.Remove(toDel);
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
