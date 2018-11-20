using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// TODO : test properly
/// </summary>
namespace Server.GameServer.Commands.Player
{
    public class EquipItemCommand : ACommand
    {
        private DataModels.Config _config;

        public EquipItemCommand(CommandArgs args, DataModels.Config config)
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var slotId = Guid.Parse(_args.Args[0]);
            //var datas = SoulManager.Instance.GetSoul(ret.ClientId);
            var inventory = SoulManager.Instance.GetInventory(ret.ClientId);
            var equipment = SoulManager.Instance.GetEquipment(ret.ClientId);

            var itemSlot = inventory.FirstOrDefault(s => s.Id.Equals(slotId));

            if (itemSlot != null)
            {
                switch (itemSlot.Type)
                {
                    case Network.Items.ItemType.Armor:
                        {
                            var item = DataManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(itemSlot.ItemId) && a.Type == Network.Items.ItemType.Armor);
                            inventory.Remove(itemSlot);
                            switch (item.Slot) // TODO : refacto, please, ugly
                            {
                                case Network.Items.ItemSlot.Head:
                                    {
                                        if (Guid.Empty.Equals(equipment.Head))
                                        {
                                            equipment.Head = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Head;
                                            equipment.Head = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.Shoulder:
                                    {
                                        if (Guid.Empty.Equals(equipment.Shoulder))
                                        {
                                            equipment.Shoulder = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Shoulder;
                                            equipment.Shoulder = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.Torso:
                                    {
                                        if (Guid.Empty.Equals(equipment.Torso))
                                        {
                                            equipment.Torso = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Torso;
                                            equipment.Torso = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.Belt:
                                    {
                                        if (Guid.Empty.Equals(equipment.Belt))
                                        {
                                            equipment.Belt = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Belt;
                                            equipment.Belt = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.Hand:
                                    {
                                        if (Guid.Empty.Equals(equipment.Hand))
                                        {
                                            equipment.Hand = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Hand;
                                            equipment.Hand = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.Pants:
                                    {
                                        if (Guid.Empty.Equals(equipment.Pants))
                                        {
                                            equipment.Pants = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Pants;
                                            equipment.Pants = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.Leg:
                                    {
                                        if (Guid.Empty.Equals(equipment.Leg))
                                        {
                                            equipment.Leg = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Leg;
                                            equipment.Leg = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.Feet:
                                    {
                                        if (Guid.Empty.Equals(equipment.Feet))
                                        {
                                            equipment.Feet = item.Id;
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Feet;
                                            equipment.Feet = item.Id;
                                            itemSlot.ItemId = toSwap;
                                            inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case Network.Items.ItemType.Bag:
                        {
                            var item = DataManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(itemSlot.ItemId) && b.Type == Network.Items.ItemType.Bag);
                            inventory.Remove(itemSlot);
                            if (Guid.Empty.Equals(equipment.Bag))
                            {
                                equipment.Bag = item.Id;
                            }
                            else
                            {
                                var bag = DataManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Bag));
                                if (bag != null)
                                {
                                    Guid toSwap = equipment.Bag;
                                    equipment.Bag = item.Id;
                                    itemSlot.ItemId = toSwap;
                                    inventory.Add(itemSlot);
                                }
                            }
                        }
                        break;
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
                        {
                            var item = DataManager.Instance.Items.FirstOrDefault(w => w.Id.Equals(itemSlot.ItemId) && w.Type == itemSlot.Type);
                            inventory.Remove(itemSlot);
                            switch (item.Slot)
                            {
                                case Network.Items.ItemSlot.OneHand:
                                    {
                                        if (Guid.Empty.Equals(equipment.WeaponRight))
                                        {
                                            equipment.WeaponRight = item.Id;
                                        }
                                        else
                                        {
                                            var rHand = DataManager.Instance.Items.FirstOrDefault(w => w.Id.Equals(equipment.WeaponRight));
                                            if (rHand != null)
                                            {
                                                if (rHand.Slot == Network.Items.ItemSlot.TwoHand)
                                                {
                                                    Guid toSwap = equipment.WeaponRight;
                                                    equipment.WeaponRight = item.Id;
                                                    itemSlot.ItemId = toSwap;
                                                    inventory.Add(itemSlot);
                                                }
                                                else
                                                {
                                                    if (Guid.Empty.Equals(equipment.WeaponLeft))
                                                    {
                                                        equipment.WeaponLeft = item.Id;
                                                    }
                                                    else
                                                    {
                                                        Guid toSwap = equipment.WeaponLeft;
                                                        equipment.WeaponLeft = equipment.WeaponRight;
                                                        equipment.WeaponRight = item.Id;
                                                        itemSlot.ItemId = toSwap;
                                                        inventory.Add(itemSlot);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case Network.Items.ItemSlot.TwoHand:
                                    {
                                        if (Guid.Empty.Equals(equipment.WeaponRight) && Guid.Empty.Equals(equipment.WeaponLeft))
                                        {
                                            equipment.WeaponRight = item.Id;
                                        }
                                        else
                                        {
                                            Guid itemId = item.Id;

                                            bool swaped = false;
                                            if (!Guid.Empty.Equals(equipment.WeaponRight))
                                            {
                                                Guid toSwap = equipment.WeaponRight;
                                                itemSlot.ItemId = toSwap;
                                                inventory.Add(itemSlot);
                                                swaped = true;
                                            }
                                            if (!Guid.Empty.Equals(equipment.WeaponLeft))
                                            {
                                                if (!swaped)
                                                {
                                                    Guid toSwap = equipment.WeaponLeft;
                                                    itemSlot.ItemId = toSwap;
                                                    inventory.Add(itemSlot);
                                                }
                                                else
                                                {
                                                    var lHand = DataManager.Instance.Items.FirstOrDefault(w => w.Id.Equals(equipment.WeaponLeft));
                                                    inventory.Add(new Network.InventorySlot
                                                    {
                                                        ItemId = equipment.WeaponLeft,
                                                        Quantity = 1,
                                                        Type = lHand.Type,
                                                        LootedAt = DateTime.Now
                                                    });
                                                    equipment.WeaponLeft = Guid.Empty;
                                                }
                                            }

                                            equipment.WeaponRight = itemId;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }

                SoulManager.Instance.UpdateEquipment(ret.ClientId, equipment);
                SoulManager.Instance.UpdateInventory(ret.ClientId, inventory);
                SoulManager.Instance.UpdateCurrentDatas(ret.ClientId, _config);
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Player.EquipItem,
                    Success = true,
                    Json = "Item equiped."
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.EquipItem,
                Success = false,
                Json = "Couldn't equip item."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
