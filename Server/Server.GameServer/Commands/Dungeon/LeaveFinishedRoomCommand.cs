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
        private DataModels.Config _config;

        public LeaveFinishedRoomCommand(CommandArgs args, DataModels.Config config)
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
                var dungeon = DataManager.Instance.Dungeons.FirstOrDefault(d => d.Id.Equals(adventure.DungeonId));

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
                        var type = (Network.Items.ItemType)Enum.Parse(typeof(Network.Items.ItemType), bag.Type);

                        switch (type)
                        {
                            case Network.Items.ItemType.Armor:
                            case Network.Items.ItemType.Bag:
                            case Network.Items.ItemType.Axe:
                            case Network.Items.ItemType.Book:
                            case Network.Items.ItemType.Bow:
                            case Network.Items.ItemType.Crossbow:
                            case Network.Items.ItemType.Dagger:
                            case Network.Items.ItemType.Fist:
                            case Network.Items.ItemType.Gun:
                            case Network.Items.ItemType.Mace:
                            case Network.Items.ItemType.Polearm:
                            case Network.Items.ItemType.Shield:
                            case Network.Items.ItemType.Staff:
                            case Network.Items.ItemType.Sword:
                            case Network.Items.ItemType.Wand:
                            case Network.Items.ItemType.Whip:
                            case Network.Items.ItemType.Jewelry:
                                {
                                    inventory.Add(new Network.InventorySlot
                                    {
                                        ItemId = bag.ItemId,
                                        Quantity = bag.Quantity,
                                        LootedAt = DateTime.Now
                                    });
                                }
                                break;
                            case Network.Items.ItemType.Junk:
                            case Network.Items.ItemType.Consumable:
                                {
                                    var exists = inventory.FirstOrDefault(i => i.ItemId.Equals(bag.ItemId));
                                    if (exists != null)
                                    {
                                        inventory.Remove(exists);
                                        exists.Quantity += bag.Quantity;
                                        inventory.Add(exists);
                                        //DataRepositories.SoulRepository.Update(soul);
                                    }
                                    else
                                    {
                                        inventory.Add(new Network.InventorySlot
                                        {
                                            ItemId = bag.ItemId,
                                            Quantity = bag.Quantity,
                                            LootedAt = DateTime.Now
                                        });
                                        //DataRepositories.SoulRepository.Update(soul);
                                    }
                                }
                                break;
                        }
                    }

                    SoulManager.Instance.UpdateCurrencies(_args.ClientId, currencies);
                    SoulManager.Instance.UpdateInventory(_args.ClientId, inventory);
                }

                // Use Experience for LevelUps
                int leveledUp = 0;
                while (soulDatas.CurrentExperience >= soulDatas.RequiredExperience)
                {
                    ++soulBaseDatas.Level;

                    currencies.Spirits += _config.SpiritsPerLevelUp;
                    currencies.Embers += _config.EmbersPerLevelUp;

                    Log.Instance.Write(Log.Level.Infos, $"{soulDatas.Name} Level up {soulDatas.CurrentExperience}/{soulDatas.RequiredExperience} => ({soulDatas.Level}).");

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

                //SoulManager.Instance.UpdateSoul(_args.ClientId, soul);
                SoulManager.Instance.UpdateCurrencies(_args.ClientId, currencies);
                SoulManager.Instance.UpdateBaseDatas(_args.ClientId, soulBaseDatas);
                SoulManager.Instance.UpdateCurrentDatas(_args.ClientId, _config);
                AdventureManager.Instance.ExitRoom(soulId);

                List<Network.AdventureState.ActionResult> json = new List<Network.AdventureState.ActionResult>();
                json.Add(new Network.AdventureState.ActionResult
                {
                    ResultType = Network.AdventureState.ActionResult.Type.EarnExperience,
                    Amount = adventureState.StackedExperience + adventureState.ExperienceReward
                });
                json.Add(new Network.AdventureState.ActionResult
                {
                    ResultType = Network.AdventureState.ActionResult.Type.EarnShards,
                    Amount = adventureState.StackedShards + adventureState.ShardReward
                });
                if (leveledUp > 0)
                {
                    json.Add(new Network.AdventureState.ActionResult
                    {
                        ResultType = Network.AdventureState.ActionResult.Type.LevelUp,
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
