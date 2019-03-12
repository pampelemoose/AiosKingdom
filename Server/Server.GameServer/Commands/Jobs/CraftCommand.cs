using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Jobs
{
    public class CraftCommand : ACommand
    {
        public CraftCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var job = SoulManager.Instance.GetJob(ret.ClientId);
            var inventory = SoulManager.Instance.GetInventory(ret.ClientId);
            var technique = (Network.JobTechnique)Enum.Parse(typeof(Network.JobTechnique), _args.Args[0]);
            var components = JsonConvert.DeserializeObject<List<Network.CraftingComponent>>(_args.Args[1]);

            var componentInvIds = components.Select(c => c.InventoryId).ToList();
            var inInventory = inventory.Where(i => componentInvIds.Contains(i.Id)).ToList();

            if (componentInvIds.Count == inInventory.Count)
            {
                inventory.RemoveAll(i => componentInvIds.Contains(i.Id));
                foreach (var invItem in inInventory)
                {
                    var component = components.FirstOrDefault(c => c.InventoryId == invItem.Id);
                    if (component != null)
                    {
                        invItem.Quantity -= component.Quantity;
                        if (invItem.Quantity > 0)
                        {
                            inventory.Add(invItem);
                        }
                    }
                }

                var item = Crafting.CraftItem(ref job, technique, components);

                int itemPoints = 1;

                if (item != null)
                {
                    inventory.Add(new Network.InventorySlot
                    {
                        Id = Guid.NewGuid(),
                        IsNew = true,
                        ItemId = item.Id,
                        Quantity = 1,
                        LootedAt = DateTime.Now
                    });

                    itemPoints += (int)item.Quality;
                }

                job.Points += (1 * itemPoints);

                SoulManager.Instance.UpdateJob(ret.ClientId, job);
                SoulManager.Instance.UpdateInventory(ret.ClientId, inventory);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Job.Craft,
                    Json = JsonConvert.SerializeObject(item),
                    Success = true
                };
                ret.Succeeded = true;
                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Job.Craft,
                Json = "Components not in inventory.",
                Success = false
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
