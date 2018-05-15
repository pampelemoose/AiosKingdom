using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientEquipItemCommand : ACommand
    {
        private DataModels.GameServer _config;

        public ClientEquipItemCommand(CommandArgs args, DataModels.GameServer config) 
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
                                /*case DataModels.Items.ArmorPart.Shoulder:
                                    {
                                        if (Guid.Empty.Equals(equipment.Shoulder))
                                        {
                                            equipment.Shoulder = item.Armor.Id;
                                            DataRepositories.RealPGRepository.Instance.Inventories.DeleteById(itemSlot.Id);
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Shoulder;
                                            equipment.Shoulder = item.Armor.Id;
                                            itemSlot.ItemId = toSwap;
                                            DataRepositories.RealPGRepository.Instance.Inventories.Update(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Torso:
                                    {
                                        if (Guid.Empty.Equals(equipment.Torso))
                                        {
                                            equipment.Torso = item.Armor.Id;
                                            DataRepositories.RealPGRepository.Instance.Inventories.DeleteById(itemSlot.Id);
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Torso;
                                            equipment.Torso = item.Armor.Id;
                                            itemSlot.ItemId = toSwap;
                                            DataRepositories.RealPGRepository.Instance.Inventories.Update(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Belt:
                                    {
                                        if (Guid.Empty.Equals(equipment.Belt))
                                        {
                                            equipment.Belt = item.Armor.Id;
                                            DataRepositories.RealPGRepository.Instance.Inventories.DeleteById(itemSlot.Id);
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Belt;
                                            equipment.Belt = item.Armor.Id;
                                            itemSlot.ItemId = toSwap;
                                            DataRepositories.RealPGRepository.Instance.Inventories.Update(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Hand:
                                    {
                                        if (Guid.Empty.Equals(equipment.Hand))
                                        {
                                            equipment.Hand = item.Armor.Id;
                                            DataRepositories.RealPGRepository.Instance.Inventories.DeleteById(itemSlot.Id);
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Hand;
                                            equipment.Hand = item.Armor.Id;
                                            itemSlot.ItemId = toSwap;
                                            DataRepositories.RealPGRepository.Instance.Inventories.Update(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Pants:
                                    {
                                        if (Guid.Empty.Equals(equipment.Pants))
                                        {
                                            equipment.Pants = item.Armor.Id;
                                            DataRepositories.RealPGRepository.Instance.Inventories.DeleteById(itemSlot.Id);
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Pants;
                                            equipment.Pants = item.Armor.Id;
                                            itemSlot.ItemId = toSwap;
                                            DataRepositories.RealPGRepository.Instance.Inventories.Update(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Leg:
                                    {
                                        if (Guid.Empty.Equals(equipment.Leg))
                                        {
                                            equipment.Leg = item.Armor.Id;
                                            DataRepositories.RealPGRepository.Instance.Inventories.DeleteById(itemSlot.Id);
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Leg;
                                            equipment.Leg = item.Armor.Id;
                                            itemSlot.ItemId = toSwap;
                                            DataRepositories.RealPGRepository.Instance.Inventories.Update(itemSlot);
                                        }
                                    }
                                    break;
                                case DataModels.Items.ArmorPart.Feet:
                                    {
                                        if (Guid.Empty.Equals(equipment.Feet))
                                        {
                                            equipment.Feet = item.Armor.Id;
                                            DataRepositories.RealPGRepository.Instance.Inventories.DeleteById(itemSlot.Id);
                                        }
                                        else
                                        {
                                            Guid toSwap = equipment.Feet;
                                            equipment.Feet = item.Armor.Id;
                                            itemSlot.ItemId = toSwap;
                                            DataRepositories.RealPGRepository.Instance.Inventories.Update(itemSlot);
                                        }
                                    }
                                    break;*/
                            }
                        }
                        break;
                    case DataModels.Items.ItemType.Bag: // TODO
                        {
                            //var item = DataRepositories.RealPGRepository.Instance.Bags.GetById(itemSlot.ItemId);
                        }
                        break;
                    case DataModels.Items.ItemType.Weapon: // TODO
                        {
                            //var item = DataRepositories.RealPGRepository.Instance.Weapons.GetById(itemSlot.ItemId);
                        }
                        break;
                }
                if (DataRepositories.SoulRepository.Update(datas))
                {
                    SoulManager.Instance.UpdateSoul(ret.ClientId, datas);
                    SoulManager.Instance.UpdateCurrentDatas(ret.ClientId, _config);
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_EquipItem,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult
                        {
                            Success = true,
                            Message = "Item equiped."
                        })
                    };
                    ret.Succeeded = true;

                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_EquipItem,
                Json = JsonConvert.SerializeObject(new Network.MessageResult
                {
                    Success = false,
                    Message = "Couldn't equip item."
                })
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
