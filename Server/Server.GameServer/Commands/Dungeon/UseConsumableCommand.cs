using Newtonsoft.Json;
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
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var slotId = Guid.Parse(_args.Args[0]);
            var enemyId = Guid.Parse(_args.Args[1]);

            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure != null)
            {
                var slotKnown = adventure.GetActualState().Bag.FirstOrDefault(s => s.InventoryId.Equals(slotId));
                if (slotKnown != null)
                {
                    var item = DataManager.Instance.Items.FirstOrDefault(c => c.Id.Equals(slotKnown.ItemId) && c.Type == Network.Items.ItemType.Consumable);
                    if (item != null)
                    {
                        var datas = SoulManager.Instance.GetDatas(ret.ClientId);

                        List<Network.AdventureState.ActionResult> consumableResult;
                        if (adventure.UseConsumable(slotKnown, item, enemyId, out consumableResult))
                        {
                            var state = adventure.GetActualState();

                            if (state.State.CurrentHealth <= 0)
                            {
                                AdventureManager.Instance.PlayerDied(soulId);

                                consumableResult.Add(new Network.AdventureState.ActionResult
                                {
                                    ResultType = Network.AdventureState.ActionResult.Type.PlayerDeath
                                });

                                ret.ClientResponse = new Network.Message
                                {
                                    Code = Network.CommandCodes.Dungeon.PlayerDied,
                                    Success = true,
                                    Json = JsonConvert.SerializeObject(consumableResult)
                                };
                                ret.Succeeded = true;
                            }
                            else
                            {
                                ret.ClientResponse = new Network.Message
                                {
                                    Code = Network.CommandCodes.Dungeon.UseConsumable,
                                    Success = true,
                                    Json = JsonConvert.SerializeObject(consumableResult)
                                };
                                ret.Succeeded = true;
                            }

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
