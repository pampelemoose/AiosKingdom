﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels.Dungeon
{
    public class ConsumableSelectionPageViewModel : BaseViewModel
    {
        public ConsumableSelectionPageViewModel(INavigation nav)
            : base(nav)
        {
        }

        public List<Models.Dungeon.ConsumableSelectionItemModel> Consumables
        {
            get
            {
                var consumableSlots = DatasManager.Instance.Soul.Inventory.Where(i => i.Type == DataModels.Items.ItemType.Consumable).ToList();
                var consumables = new List<Models.Dungeon.ConsumableSelectionItemModel>();
                foreach (var slot in consumableSlots)
                {
                    var item = DatasManager.Instance.Consumables.FirstOrDefault(b => b.ItemId.Equals(slot.ItemId));

                    if (item != null)
                    {
                        consumables.Add(new Models.Dungeon.ConsumableSelectionItemModel
                        {
                            SlotId = slot.Id,
                            Quantity = slot.Quantity,
                            Consumable = item
                        });
                    }
                }

                return consumables;
            }
        }

        private Models.Dungeon.ConsumableSelectionItemModel _selectedConsumable;
        public Models.Dungeon.ConsumableSelectionItemModel SelectedConsumable
        {
            get { return _selectedConsumable; }
            set
            {
                _selectedConsumable = value;
                _validateAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private ICommand _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                _navigation.PopModalAsync();
            }));

        private Command _validateAction;
        public ICommand ValidateAction =>
            _validateAction ?? (_validateAction = new Command(() =>
            {
                MessagingCenter.Send(this, MessengerCodes.DungeonSelectConsumableEnded, _selectedConsumable);
                _navigation.PopModalAsync();
            }, () => { return _selectedConsumable != null; }));
    }
}
