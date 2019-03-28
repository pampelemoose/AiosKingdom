using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// TODO : Changes here are quite heavy. Need to test thorougly
/// </summary>
namespace Server.GameServer.Commands.Dungeon
{
    public class LeaveFinishedRoomCommand : ACommand
    {
        private DataModels.Town _config;

        public LeaveFinishedRoomCommand(CommandArgs args, DataModels.Town config)
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure.IsCleared)
            {
                var soulDatas = SoulManager.Instance.GetDatas(_args.ClientId);
                var soulBaseDatas = SoulManager.Instance.GetBaseDatas(_args.ClientId);
                var kingdom = DataRepositories.KingdomRepository.GetById(_config.KingdomId);
                var currencies = SoulManager.Instance.GetCurrencies(_args.ClientId);
                var inventory = SoulManager.Instance.GetInventory(_args.ClientId);

                var adventureState = adventure.GetActualState();
                var dungeon = DataManager.Instance.Dungeons.FirstOrDefault(d => d.Id.Equals(adventure.AdventureId));

                // We get exp and loots and everything only if we exit finished adventure.
                if (adventureState.IsExit)
                {
                    if (soulDatas.Level < kingdom.CurrentMaxLevel)
                    {
                        soulBaseDatas.CurrentExperience += adventureState.StackedExperience;
                        soulBaseDatas.CurrentExperience += dungeon.ExperienceReward;
                    }

                    currencies.Shards += adventureState.StackedShards;
                    currencies.Shards += dungeon.ShardReward;

                    foreach (var bag in adventureState.Bag)
                    {
                        var exists = inventory.FirstOrDefault(i => i.ItemId.Equals(bag.ItemId));
                        if (exists != null)
                        {
                            inventory.Remove(exists);
                            exists.Quantity += bag.Quantity;
                            inventory.Add(exists);
                        }
                        else
                        {
                            inventory.Add(new Network.InventorySlot
                            {
                                Id = bag.InventoryId,
                                IsNew = true,
                                ItemId = bag.ItemId,
                                Quantity = bag.Quantity,
                                LootedAt = DateTime.Now
                            });
                        }
                    }

                    SoulManager.Instance.UpdateCurrencies(_args.ClientId, currencies);
                    SoulManager.Instance.UpdateInventory(_args.ClientId, inventory);

                    // Use Experience for LevelUps
                    int leveledUp = 0;
                    while (soulDatas.CurrentExperience >= soulDatas.RequiredExperience)
                    {
                        ++soulBaseDatas.Level;

                        currencies.Spirits += _config.SpiritsPerLevelUp;
                        currencies.Embers += _config.EmbersPerLevelUp;

                        Log.Instance.Write(Log.Type.Log, Log.Level.Infos, $"{soulDatas.Name} Level up {soulDatas.CurrentExperience}/{soulDatas.RequiredExperience} => ({soulDatas.Level}).");

                        soulBaseDatas.CurrentExperience -= soulDatas.RequiredExperience;

                        //SoulManager.Instance.UpdateSoul(_args.ClientId, soul);
                        SoulManager.Instance.UpdateBaseDatas(_args.ClientId, soulBaseDatas);

                        SoulManager.Instance.UpdateCurrentDatas(_args.ClientId, _config);
                        soulDatas = SoulManager.Instance.GetDatas(_args.ClientId);
                        Console.WriteLine($"{soulDatas.Name} Level up({soulDatas.Level}).");
                        leveledUp++;

                        // Check the Kingdom maxLevel count
                        if (soulDatas.Level == kingdom.CurrentMaxLevel)
                        {
                            kingdom.CurrentMaxLevelCount++;
                            DataRepositories.KingdomRepository.Update(kingdom);
                            soulBaseDatas.CurrentExperience = 0;
                            break;
                        }
                    }

                    var locks = SoulManager.Instance.GetAdventureLocks(_args.ClientId);
                    if (locks == null) locks = new List<Network.AdventureUnlocked>();
                    if (!locks.Any(l => l.AdventureId.Equals(adventure.AdventureId)))
                    {
                        locks.Add(new Network.AdventureUnlocked
                        {
                            AdventureId = adventure.AdventureId,
                            UnlockedAt = DateTime.Now
                        });
                        SoulManager.Instance.UpdateAdventureLocks(_args.ClientId, locks);
                    }

                    //SoulManager.Instance.UpdateSoul(_args.ClientId, soul);
                    SoulManager.Instance.UpdateCurrencies(_args.ClientId, currencies);
                    SoulManager.Instance.UpdateBaseDatas(_args.ClientId, soulBaseDatas);
                    SoulManager.Instance.UpdateCurrentDatas(_args.ClientId, _config);

                    AdventureManager.Instance.ExitRoom(soulId);

                    List<Network.ActionResult> json = new List<Network.ActionResult>();
                    json.Add(new Network.ActionResult
                    {
                        ResultType = Network.ActionResult.Type.EarnExperience,
                        Amount = adventureState.StackedExperience + adventureState.ExperienceReward
                    });
                    json.Add(new Network.ActionResult
                    {
                        ResultType = Network.ActionResult.Type.EarnShards,
                        Amount = adventureState.StackedShards + adventureState.ShardReward
                    });
                    if (leveledUp > 0)
                    {
                        json.Add(new Network.ActionResult
                        {
                            ResultType = Network.ActionResult.Type.LevelUp,
                            Amount = leveledUp
                        });
                    }

                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Dungeon.LeaveFinishedRoom,
                        Success = true,
                        Json = JsonConvert.SerializeObject(json)
                    };
                    ret.Succeeded = true;

                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.LeaveFinishedRoom,
                Success = false,
                Json = "Room not cleared, can't exit."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
