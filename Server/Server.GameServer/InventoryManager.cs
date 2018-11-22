using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public static class InventoryManager
    {
        public static void AddItemToInventory(Guid clientId, Guid itemId, int quantity = 1)
        {
            var inventory = SoulManager.Instance.GetInventory(clientId);
            var item = DataManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(itemId));

            if (inventory != null && item != null)
            {
                switch (item.Type)
                {
                    case Network.Items.ItemType.Armor:
                    case Network.Items.ItemType.Axe:
                    case Network.Items.ItemType.Bag:
                    case Network.Items.ItemType.Book:
                    case Network.Items.ItemType.Bow:
                    case Network.Items.ItemType.Crossbow:
                    case Network.Items.ItemType.Dagger:
                    case Network.Items.ItemType.Fist:
                    case Network.Items.ItemType.Gun:
                    case Network.Items.ItemType.Jewelry:
                    case Network.Items.ItemType.Mace:
                    case Network.Items.ItemType.Polearm:
                    case Network.Items.ItemType.Shield:
                    case Network.Items.ItemType.Staff:
                    case Network.Items.ItemType.Sword:
                    case Network.Items.ItemType.Wand:
                    case Network.Items.ItemType.Whip:
                        inventory.Add(new Network.InventorySlot
                        {
                            ItemId = item.Id,
                            Quantity = 1,
                            LootedAt = DateTime.Now
                        });
                        break;
                    case Network.Items.ItemType.Consumable:
                    case Network.Items.ItemType.Junk:
                        var exists = inventory.FirstOrDefault(i => i.ItemId.Equals(item.Id));
                        if (exists != null)
                        {
                            inventory.Remove(exists);
                            exists.Quantity += quantity;
                            inventory.Add(exists);
                        }
                        else
                        {
                            inventory.Add(new Network.InventorySlot
                            {
                                ItemId = item.Id,
                                Quantity = quantity,
                                LootedAt = DateTime.Now
                            });
                        }
                        break;
                }
                SoulManager.Instance.UpdateInventory(clientId, inventory);
            }
        }
    }
}
