using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Adventure
{
    public class StartCommand : ACommand
    {
        public StartCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var knowledges = SoulManager.Instance.GetKnowledges(_args.ClientId);
            var inventory = SoulManager.Instance.GetInventory(_args.ClientId);
            var datas = SoulManager.Instance.GetDatas(_args.ClientId);
            var unlocks = SoulManager.Instance.GetAdventureLocks(_args.ClientId);
            var dungeonId = Guid.Parse(_args.Args[0]);
            var bagItems = JsonConvert.DeserializeObject<List<Network.AdventureState.BagItem>>(_args.Args[1]);

            if (!AdventureManager.Instance.IsUnlocked(dungeonId, unlocks))
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.Start,
                    Success = false,
                    Json = "Couldn't enter because you didn't unlock this adventure yet."
                };
                ret.Succeeded = true;

                return ret;
            }

            var adventure = AdventureManager.Instance.Start(soulId, datas, dungeonId, knowledges, bagItems);

            if (adventure == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.Start,
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

            var adventureState = adventure.GetActualState();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Adventure.Start,
                Success = true,
                Json = JsonConvert.SerializeObject(adventureState)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
