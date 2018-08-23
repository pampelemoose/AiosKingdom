using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var soulDatas = SoulManager.Instance.GetDatas(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soul.Id);

            if (adventure.IsCleared)
            {
                var adventureState = adventure.GetActualState();
                var dungeon = DataRepositories.DungeonRepository.GetById(adventure.DungeonId);

                if (adventureState.IsExit)
                {
                    soul.CurrentExperience += adventureState.StackedExperience;
                    soul.Shards += adventureState.StackedShards;
                    soul.CurrentExperience += dungeon.ExperienceReward;
                    soul.Shards += dungeon.ShardReward;

                    foreach (var bag in adventureState.Bag)
                    {
                        var type = (DataModels.Items.ItemType)Enum.Parse(typeof(DataModels.Items.ItemType), bag.Type);

                        switch (type)
                        {
                            case DataModels.Items.ItemType.Armor:
                            case DataModels.Items.ItemType.Bag:
                            case DataModels.Items.ItemType.Weapon:
                            case DataModels.Items.ItemType.Jewelry:
                                {
                                    soul.Inventory.Add(new DataModels.InventorySlot
                                    {
                                        ItemId = bag.ItemId,
                                        Type = type,
                                        Quantity = bag.Quantity,
                                        SoulId = soul.Id,
                                        LootedAt = DateTime.Now
                                    });
                                }
                                break;
                            case DataModels.Items.ItemType.Consumable:
                                {
                                    var exists = soul.Inventory.FirstOrDefault(i => i.ItemId.Equals(bag.ItemId));
                                    if (exists != null)
                                    {
                                        soul.Inventory.Remove(exists);
                                        exists.Quantity += bag.Quantity;
                                        soul.Inventory.Add(exists);
                                        DataRepositories.SoulRepository.Update(soul);
                                    }
                                    else
                                    {
                                        soul.Inventory.Add(new DataModels.InventorySlot
                                        {
                                            ItemId = bag.ItemId,
                                            Type = type,
                                            Quantity = bag.Quantity,
                                            SoulId = soul.Id,
                                            LootedAt = DateTime.Now
                                        });
                                        DataRepositories.SoulRepository.Update(soul);
                                    }
                                }
                                break;
                        }
                    }
                }

                while (soul.CurrentExperience >= soulDatas.RequiredExperience)
                {
                    ++soul.Level;

                    soul.Spirits += _config.SpiritsPerLevelUp;
                    soul.Embers += _config.EmbersPerLevelUp;

                    Log.Instance.Write(Log.Level.Infos, $"{soul.Name} Level up {soul.CurrentExperience}/{soulDatas.RequiredExperience} => ({soul.Level}).");

                    soul.CurrentExperience -= soulDatas.RequiredExperience;

                    SoulManager.Instance.UpdateSoul(_args.ClientId, soul);
                    SoulManager.Instance.UpdateCurrentDatas(_args.ClientId, _config);
                    soulDatas = SoulManager.Instance.GetDatas(_args.ClientId);
                    Console.WriteLine($"{soul.Name} Level up({soul.Level}).");
                }

                SoulManager.Instance.UpdateSoul(_args.ClientId, soul);
                SoulManager.Instance.UpdateCurrentDatas(_args.ClientId, _config);
                AdventureManager.Instance.ExitRoom(soul.Id);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.LeaveFinishedRoom,
                    Success = true,
                    Json = $"Exited the dungeon. you earned {adventureState.StackedExperience + adventureState.ExperienceReward} experience and {adventureState.StackedShards + adventureState.ShardReward} shards."
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
