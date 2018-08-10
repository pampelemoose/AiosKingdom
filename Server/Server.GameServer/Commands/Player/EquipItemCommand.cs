using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var datas = SoulManager.Instance.GetSoul(ret.ClientId);

            var itemSlot = datas.Inventory.FirstOrDefault(s => s.Id.Equals(slotId));

            if (itemSlot != null)
            {
                switch (itemSlot.Type)
                {
                    case DataModels.Items.ItemType.Armor:
                        {
                            var item = DataRepositories.ArmorRepository.GetById(itemSlot.ItemId);
                            datas.Inventory.Remove(itemSlot);
                            switch (item.Part) // TODO : refacto, please, ugly
                            {
                                case DataModels.Items.ArmorPart.Head:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Head))
                                        {
                                            datas.Equipment.Head = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Head;
                                            datas.Equipment.Head = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Shoulder:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Shoulder))
                                        {
                                            datas.Equipment.Shoulder = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Shoulder;
                                            datas.Equipment.Shoulder = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Torso:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Torso))
                                        {
                                            datas.Equipment.Torso = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Torso;
                                            datas.Equipment.Torso = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Belt:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Belt))
                                        {
                                            datas.Equipment.Belt = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Belt;
                                            datas.Equipment.Belt = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Hand:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Hand))
                                        {
                                            datas.Equipment.Hand = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Hand;
                                            datas.Equipment.Hand = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Pants:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Pants))
                                        {
                                            datas.Equipment.Pants = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Pants;
                                            datas.Equipment.Pants = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Leg:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Leg))
                                        {
                                            datas.Equipment.Leg = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Leg;
                                            datas.Equipment.Leg = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Feet:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.Feet))
                                        {
                                            datas.Equipment.Feet = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid toSwap = datas.Equipment.Feet;
                                            datas.Equipment.Feet = item.ItemId;
                                            itemSlot.ItemId = toSwap;
                                            datas.Inventory.Add(itemSlot);
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case DataModels.Items.ItemType.Bag:
                        {
                            var item = DataRepositories.BagRepository.GetById(itemSlot.ItemId);
                            datas.Inventory.Remove(itemSlot);
                            if (Guid.Empty.Equals(datas.Equipment.Bag))
                            {
                                datas.Equipment.Bag = item.ItemId;
                            }
                            else
                            {
                                var bag = DataRepositories.BagRepository.GetById(datas.Equipment.Bag);
                                if (bag != null)
                                {
                                    Guid toSwap = datas.Equipment.Bag;
                                    datas.Equipment.Bag = item.ItemId;
                                    itemSlot.ItemId = toSwap;
                                    datas.Inventory.Add(itemSlot);
                                }
                            }
                        }
                        break;
                    case DataModels.Items.ItemType.Weapon:
                        {
                            var item = DataRepositories.WeaponRepository.GetById(itemSlot.ItemId);
                            datas.Inventory.Remove(itemSlot);
                            switch (item.HandlingType)
                            {
                                case DataModels.Items.HandlingType.OneHand:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.WeaponRight))
                                        {
                                            datas.Equipment.WeaponRight = item.ItemId;
                                        }
                                        else
                                        {
                                            var rHand = DataRepositories.WeaponRepository.GetById(datas.Equipment.WeaponRight);
                                            if (rHand != null)
                                            {
                                                if (rHand.HandlingType == DataModels.Items.HandlingType.TwoHand)
                                                {
                                                    Guid toSwap = datas.Equipment.WeaponRight;
                                                    datas.Equipment.WeaponRight = item.ItemId;
                                                    itemSlot.ItemId = toSwap;
                                                    datas.Inventory.Add(itemSlot);
                                                }
                                                else
                                                {
                                                    if (Guid.Empty.Equals(datas.Equipment.WeaponLeft))
                                                    {
                                                        datas.Equipment.WeaponLeft = item.ItemId;
                                                    }
                                                    else
                                                    {
                                                        Guid toSwap = datas.Equipment.WeaponLeft;
                                                        datas.Equipment.WeaponLeft = datas.Equipment.WeaponRight;
                                                        datas.Equipment.WeaponRight = item.ItemId;
                                                        itemSlot.ItemId = toSwap;
                                                        datas.Inventory.Add(itemSlot);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case DataModels.Items.HandlingType.TwoHand:
                                    {
                                        if (Guid.Empty.Equals(datas.Equipment.WeaponRight) && Guid.Empty.Equals(datas.Equipment.WeaponLeft))
                                        {
                                            datas.Equipment.WeaponRight = item.ItemId;
                                        }
                                        else
                                        {
                                            Guid itemId = item.ItemId;

                                            bool swaped = false;
                                            if (!Guid.Empty.Equals(datas.Equipment.WeaponRight))
                                            {
                                                Guid toSwap = datas.Equipment.WeaponRight;
                                                itemSlot.ItemId = toSwap;
                                                datas.Inventory.Add(itemSlot);
                                                swaped = true;
                                            }
                                            if (!Guid.Empty.Equals(datas.Equipment.WeaponLeft))
                                            {
                                                if (!swaped)
                                                {
                                                    Guid toSwap = datas.Equipment.WeaponLeft;
                                                    itemSlot.ItemId = toSwap;
                                                    datas.Inventory.Add(itemSlot);
                                                }
                                                else
                                                {
                                                    datas.Inventory.Add(new DataModels.InventorySlot
                                                    {
                                                        ItemId = datas.Equipment.WeaponLeft,
                                                        Quantity = 1,
                                                        SoulId = itemSlot.SoulId,
                                                        Type = DataModels.Items.ItemType.Weapon,
                                                        LootedAt = DateTime.Now
                                                    });
                                                    datas.Equipment.WeaponLeft = Guid.Empty;
                                                }
                                            }

                                            datas.Equipment.WeaponRight = itemId;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
                if (DataRepositories.SoulRepository.Update(datas))
                {
                    SoulManager.Instance.UpdateSoul(ret.ClientId, datas);
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
