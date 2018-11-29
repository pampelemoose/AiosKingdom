using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class EnterCommand : ACommand
    {
        public EnterCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var knowledges = SoulManager.Instance.GetKnowledges(_args.ClientId);
            var inventory = SoulManager.Instance.GetInventory(_args.ClientId);
            var datas = SoulManager.Instance.GetDatas(_args.ClientId);
            var dungeonId = Guid.Parse(_args.Args[0]);
            var bagItems = JsonConvert.DeserializeObject<List<Network.AdventureState.BagItem>>(_args.Args[1]);

            if (knowledges.Count > 0)
            {
                var adventure = AdventureManager.Instance.OpenRoom(soulId, datas, dungeonId, bagItems);

                if (adventure == null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Dungeon.Enter,
                        Success = false,
                        Json = "Couldn't enter because you didn't leave properly last time."
                    };
                    ret.Succeeded = true;

                    return ret;
                }

                foreach (var bagItem in bagItems)
                {
                    var exists = inventory.FirstOrDefault(i => i.Id.Equals(bagItem.InventoryId));
                    if (exists != null)
                    {
                        inventory.Remove(exists);
                        exists.Quantity -= bagItem.Quantity;
                        if (exists.Quantity > 0)
                        {
                            inventory.Add(exists);
                        }
                    }
                }

                SoulManager.Instance.UpdateInventory(_args.ClientId, inventory);

                if (adventure.RoomNumber == 0)
                {
                    adventure.SetPlayerState(SoulManager.Instance.GetDatas(_args.ClientId));
                }

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.Enter,
                    Success = true,
                    Json = "Entered the dungeon."
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.Enter,
                Success = false,
                Json = "You must have learned at least one skill."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
