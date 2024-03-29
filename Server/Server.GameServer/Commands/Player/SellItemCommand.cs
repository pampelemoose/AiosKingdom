﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// TODO : test
/// </summary>
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
            var currencies = SoulManager.Instance.GetCurrencies(ret.ClientId);
            var inventory = SoulManager.Instance.GetInventory(ret.ClientId);
            var inventorySlotId = Guid.Parse(_args.Args[0]);

            var inventorySlot = inventory.FirstOrDefault(i => i.Id.Equals(inventorySlotId));

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

            inventory.Remove(inventorySlot);
            inventorySlot.Quantity--;

            if (inventorySlot.Quantity > 0)
            {
                inventory.Add(inventorySlot);
            }


            var exists = DataManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(inventorySlot.ItemId));
            if (exists == null)
            {
                return objectNotFound(ret);
            }

            currencies.Shards += exists.SellingPrice;

            SoulManager.Instance.UpdateInventory(ret.ClientId, inventory);
            SoulManager.Instance.UpdateCurrencies(ret.ClientId, currencies);

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
