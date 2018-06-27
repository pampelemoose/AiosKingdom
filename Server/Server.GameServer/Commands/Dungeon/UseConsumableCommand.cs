using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class UseConsumableCommand : ACommand
    {
        public UseConsumableCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var slotId = Guid.Parse(_args.Args[0]);
            var enemyId = Guid.Parse(_args.Args[1]);

            var adventure = AdventureManager.Instance.GetAdventure(soul);

            if (adventure != null)
            {
                var slotKnown = soul.Inventory.FirstOrDefault(s => s.Id.Equals(slotId));
                if (slotKnown != null)
                {
                    var item = DataRepositories.ConsumableRepository.GetById(slotKnown.ItemId);
                    if (item != null)
                    {
                        soul.Inventory.Remove(slotKnown);

                        var datas = SoulManager.Instance.GetDatas(ret.ClientId);

                        if (adventure.UseConsumable(item, SoulManager.Instance.GetDatas(ret.ClientId), enemyId))
                        {
                            --slotKnown.Quantity;

                            if (slotKnown.Quantity > 0)
                            {
                                soul.Inventory.Add(slotKnown);
                            }

                            SoulManager.Instance.UpdateSoul(ret.ClientId, soul);

                            ret.ClientResponse = new Network.Message
                            {
                                Code = Network.CommandCodes.Dungeon.UseConsumable,
                                Success = true,
                                Json = "Consumable successfully used."
                            };
                            ret.Succeeded = true;

                            return ret;
                        }
                    }
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.UseConsumable,
                Success = false,
                Json = "Failed to use consumable."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
