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
                            var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(itemSlot.ItemId));
                            inventory.Remove(itemSlot);
                            switch (item.Part) // TODO : refacto, please, ugly
                            {
                                case Network.Items.ArmorPart.Head:
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
                                case Network.Items.ArmorPart.Shoulder:
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
                                case Network.Items.ArmorPart.Torso:
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
                                case Network.Items.ArmorPart.Belt:
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
                                case Network.Items.ArmorPart.Hand:
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
                                case Network.Items.ArmorPart.Pants:
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
                                case Network.Items.ArmorPart.Leg:
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
                                case Network.Items.ArmorPart.Feet:
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
                            var item = DataManager.Instance.Bags.FirstOrDefault(b => b.Id.Equals(itemSlot.ItemId));
                            inventory.Remove(itemSlot);
                            if (Guid.Empty.Equals(equipment.Bag))
                            {
                                equipment.Bag = item.Id;
                            }
                            else
                            {
                                var bag = DataManager.Instance.Bags.FirstOrDefault(b => b.Id.Equals(equipment.Bag));
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
                    case Network.Items.ItemType.Weapon:
                        {
                            var item = DataManager.Instance.Weapons.FirstOrDefault(w => w.Id.Equals(itemSlot.ItemId));
                            inventory.Remove(itemSlot);
                            switch (item.HandlingType)
                            {
                                case Network.Items.HandlingType.OneHand:
                                    {
                                        if (Guid.Empty.Equals(equipment.WeaponRight))
                                        {
                                            equipment.WeaponRight = item.Id;
                                        }
                                        else
                                        {
                                            var rHand = DataManager.Instance.Weapons.FirstOrDefault(w => w.Id.Equals(equipment.WeaponRight));
                                            if (rHand != null)
                                            {
                                                if (rHand.HandlingType == Network.Items.HandlingType.TwoHand)
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
                                case Network.Items.HandlingType.TwoHand:
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
                                                    inventory.Add(new Network.InventorySlot
                                                    {
                                                        ItemId = equipment.WeaponLeft,
                                                        Quantity = 1,
                                                        Type = Network.Items.ItemType.Weapon,
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
