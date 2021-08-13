using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace DataRepositories
{
    public static class SoulRepository
    {
        public static List<DataModels.Soul> GetSoulsByAppUserId(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Souls
                    .Where(s => s.AppUserId.Equals(id)).ToList();
            }
        }

        public static DataModels.Soul GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Souls
                    .Include(s => s.Equipment)
                    .Include(s => s.Inventory)
                    .Include(s => s.Knowledge)
                    .Include(a => a.Knowledge.Select(i => i.Talents))
                    .Include(s => s.AdventureLocks)
                    .FirstOrDefault(s => s.Id.Equals(id));
            }
        }

        public static bool CreateSoul(DataModels.Soul soul)
        {
            using (var context = new AiosKingdomContext())
            {
                var nameUsed = context.Souls.FirstOrDefault(s => s.Name.Equals(soul.Name));
                if (nameUsed != null)
                {
                    return false;
                }

                if (soul.Id.Equals(Guid.Empty))
                {
                    return false;
                }

                context.Souls.Add(soul);
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

        public static bool Update(DataModels.Soul soul)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Souls
                    .Include(s => s.Equipment)
                    .Include(s => s.Inventory)
                    .Include(s => s.Knowledge)
                    .Include(a => a.Knowledge.Select(i => i.Talents))
                    .Include(s => s.AdventureLocks)
                    .FirstOrDefault(s => s.Id.Equals(soul.Id));

                if (online == null) return false;

                online.TimePlayed = soul.TimePlayed;
                online.Level = soul.Level;

                // EQUIPMENT
                online.Equipment.Head = soul.Equipment.Head;
                online.Equipment.Shoulder = soul.Equipment.Shoulder;
                online.Equipment.Torso = soul.Equipment.Torso;
                online.Equipment.Belt = soul.Equipment.Belt;
                online.Equipment.Hand = soul.Equipment.Hand;
                online.Equipment.Pants = soul.Equipment.Pants;
                online.Equipment.Leg = soul.Equipment.Leg;
                online.Equipment.Feet = soul.Equipment.Feet;

                online.Equipment.Bag = soul.Equipment.Bag;

                online.Equipment.WeaponRight = soul.Equipment.WeaponRight;
                online.Equipment.WeaponLeft = soul.Equipment.WeaponLeft;

                // INVENTORY
                var oldInv = online.Inventory;
                online.Inventory = new List<DataModels.InventorySlot>();
                foreach (var item in soul.Inventory)
                {
                    if (Guid.Empty.Equals(item.Id))
                    {
                        item.Id = Guid.NewGuid();
                        online.Inventory.Add(item);
                    }
                    else
                    {
                        var inv = context.Inventories.FirstOrDefault(i => i.Id.Equals(item.Id));
                        inv.Quantity = item.Quantity;
                        inv.ItemVid = item.ItemVid;
                        online.Inventory.Add(inv);
                        oldInv.Remove(oldInv.FirstOrDefault(o => o.Id.Equals(item.Id)));
                    }
                }

                foreach (var toDel in oldInv)
                {
                    context.Inventories.Remove(toDel);
                }

                // KNOWLEDGE
                var oldKno = online.Knowledge;
                online.Knowledge = new List<DataModels.Knowledge>();
                foreach (var kno in soul.Knowledge)
                {
                    if (Guid.Empty.Equals(kno.Id))
                    {
                        kno.Id = Guid.NewGuid();

                        foreach (var talent in kno.Talents)
                        {
                            talent.Id = Guid.NewGuid();
                        }

                        online.Knowledge.Add(kno);
                    }
                    else
                    {
                        var know = context.Knowledges.FirstOrDefault(i => i.Id.Equals(kno.Id));
                        know.TalentPoints = kno.TalentPoints;

                        // TALENTS
                        var oldTalents = know.Talents;
                        know.Talents = new List<DataModels.TalentUnlocked>();
                        foreach (var tal in kno.Talents)
                        {
                            if (Guid.Empty.Equals(tal.Id))
                            {
                                tal.Id = Guid.NewGuid();
                                tal.KnowledgeId = kno.Id;
                                know.Talents.Add(tal);
                            }
                            else
                            {
                                var onlineTalent = context.TalentUnlocked.FirstOrDefault(i => i.Id.Equals(tal.Id));
                                onlineTalent.TalentId = tal.TalentId;
                                onlineTalent.UnlockedAt = tal.UnlockedAt;
                                know.Talents.Add(onlineTalent);
                                oldTalents.Remove(oldTalents.FirstOrDefault(o => o.Id.Equals(tal.Id)));
                            }
                        }

                        foreach (var toDel in oldTalents)
                        {
                            context.TalentUnlocked.Remove(toDel);
                        }

                        online.Knowledge.Add(know);
                        oldKno.Remove(oldKno.FirstOrDefault(o => o.Id.Equals(kno.Id)));
                    }
                }

                foreach (var toDel in oldKno)
                {
                    context.Knowledges.Remove(toDel);
                }

                // PROGRESS
                var oldLocks = online.AdventureLocks;
                online.AdventureLocks = new List<DataModels.AdventureUnlocked>();
                foreach (var pro in soul.AdventureLocks)
                {
                    if (Guid.Empty.Equals(pro.Id))
                    {
                        pro.Id = Guid.NewGuid();
                        online.AdventureLocks.Add(pro);
                    }
                    else
                    {
                        var progress = context.AdventureUnlocked.FirstOrDefault(i => i.Id.Equals(pro.Id));
                        // TODO : Any changes to the objects here.
                        online.AdventureLocks.Add(progress);
                        oldLocks.Remove(oldLocks.FirstOrDefault(o => o.Id.Equals(pro.Id)));
                    }
                }

                foreach (var toDel in oldLocks)
                {
                    context.AdventureUnlocked.Remove(toDel);
                }

                online.CurrentExperience = soul.CurrentExperience;

                // STATS
                online.Stamina = soul.Stamina;
                online.Energy = soul.Energy;
                online.Strength = soul.Strength;
                online.Agility = soul.Agility;
                online.Intelligence = soul.Intelligence;
                online.Wisdom = soul.Wisdom;

                // MONEY VALUES
                online.StatPoints = soul.StatPoints;
                online.Shards = soul.Shards;
                online.Bits = soul.Bits;

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
