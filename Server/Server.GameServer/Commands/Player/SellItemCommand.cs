using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class SellItemCommand : ACommand
    {
        public SellItemCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(ret.ClientId);
            var inventorySlotId = Guid.Parse(_args.Args[0]);

            var inventorySlot = soul.Inventory.FirstOrDefault(i => i.Id.Equals(inventorySlotId));

            if (inventorySlot == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.SellItem,
                    Success = false,
                    Json = "Couldn't find item in Inventory."
                };
                ret.Succeeded = true;
                return ret;
            }

            soul.Inventory.Remove(inventorySlot);
            inventorySlot.Quantity--;

            if (inventorySlot.Quantity > 0)
            {
                soul.Inventory.Add(inventorySlot);
            }


            switch (inventorySlot.Type)
            {
                case DataModels.Items.ItemType.Armor:
                    {
                        var exists = DataRepositories.ArmorRepository.GetById(inventorySlot.ItemId);
                        if (exists == null)
                        {
                            return objectNotFound(ret);
                        }

                        soul.Shards += exists.SellingPrice;
                    }
                    break;
                case DataModels.Items.ItemType.Bag:
                    {
                        var exists = DataRepositories.BagRepository.GetById(inventorySlot.ItemId);
                        if (exists == null)
                        {
                            return objectNotFound(ret);
                        }

                        soul.Shards += exists.SellingPrice;
                    }
                    break;
                case DataModels.Items.ItemType.Weapon:
                    {
                        var exists = DataRepositories.WeaponRepository.GetById(inventorySlot.ItemId);
                        if (exists == null)
                        {
                            return objectNotFound(ret);
                        }

                        soul.Shards += exists.SellingPrice;
                    }
                    break;
                case DataModels.Items.ItemType.Jewelry:
                    {
                    }
                    break;
                case DataModels.Items.ItemType.Consumable:
                    {
                        var exists = DataRepositories.ConsumableRepository.GetById(inventorySlot.ItemId);
                        if (exists == null)
                        {
                            return objectNotFound(ret);
                        }

                        soul.Shards += exists.SellingPrice;
                    }
                    break;
            }

            SoulManager.Instance.UpdateSoul(ret.ClientId, soul);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.SellItem,
                Success = true,
                Json = $"Sold item successfully. Please check your currencies."
            };
            ret.Succeeded = true;
            return ret;
        }

        private CommandResult objectNotFound(CommandResult ret)
        {
            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.SellItem,
                Success = false,
                Json = "Item not found."
            };
            ret.Succeeded = true;
            return ret;
        }
    }
}
