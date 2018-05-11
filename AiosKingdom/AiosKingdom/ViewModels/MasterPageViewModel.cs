using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.ViewModels
{
    public class MasterPageViewModel : BaseViewModel
    {
        public MasterPageViewModel()
            : base(null)
        {
            _menus = new List<MasterPageItem>();

            /*DatasManager.Instance.CurrentSoulChanged += () =>
            {
                _menus = new List<MasterPageItem>();

                _menus.Add(new MasterPageItem
                {
                    TargetType = typeof(Views.DashboardPage),
                    Title = "Dashboard"
                });
                _menus.Add(new MasterPageItem
                {
                    TargetType = typeof(Views.InventoryPage),
                    Title = "Inventory"
                });
                _menus.Add(new MasterPageItem
                {
                    TargetType = typeof(Views.SpiritPillsPage),
                    Title = "Spirit Pills"
                });
                _menus.Add(new MasterPageItem
                {
                    TargetType = typeof(Views.ArmouryPage),
                    Title = "Arnoury"
                });
                _menus.Add(new MasterPageItem
                {
                    TargetType = typeof(Views.DrugstorePage),
                    Title = "Drugstore"
                });
                _menus.Add(new MasterPageItem
                {
                    TargetType = typeof(Views.BookstorePage),
                    Title = "Bookstore"
                });
                _menus.Add(new MasterPageItem
                {
                    TargetType = typeof(Views.MarketPage),
                    Title = "Market"
                });

                NotifyPropertyChanged(nameof(Menus));
                NotifyPropertyChanged(nameof(ConnectedToSoul));
                NotifyPropertyChanged(nameof(Soul));
            };

            DatasManager.Instance.CurrentDatasChanged += () =>
            {
                NotifyPropertyChanged(nameof(Datas));
            };*/

            _menus.Add(new MasterPageItem
            {
                TargetType = typeof(Views.DashboardPage),
                Title = "Dashboard"
            });
        }

        public event Action<Type> PageChangeTriggered;

        private List<MasterPageItem> _menus;
        public List<MasterPageItem> Menus => _menus;

        public MasterPageItem SelectedItem
        {
            get { return null; }
            set
            {
                if (value != null)
                    PageChangeTriggered?.Invoke(value.TargetType);

                NotifyPropertyChanged();
            }
        }

        /*public bool ConnectedToSoul
        {
            get { return DatasManager.Instance.CurrentSoul != null; }
        }

        public DataModels.Soul Soul
        {
            get { return DatasManager.Instance.CurrentSoul; }
        }

        public Network.SoulDatas Datas
        {
            get { return DatasManager.Instance.CurrentDatas; }
        }

        private bool _onSoulList;
        public bool OnSoulList
        {
            get { return _onSoulList; }
            set
            {
                _onSoulList = !value;
                NotifyPropertyChanged();
            }
        }

        public event Action BackToSoulTriggered;
        private ICommand _backToSoulListAction;
        public ICommand BackToSoulListAction =>
        _backToSoulListAction ?? (_backToSoulListAction = new Command(() =>
        {
            NetworkManager.Instance.DisconnectionFromSoul();

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (DatasManager.Instance.CurrentSoul == null)
                {
                    DatasManager.Instance.SoftReset();
                    _menus = new List<MasterPageItem>();
                    NotifyPropertyChanged(nameof(Menus));
                    NotifyPropertyChanged(nameof(ConnectedToSoul));
                    NotifyPropertyChanged(nameof(Soul));
                    NotifyPropertyChanged(nameof(Datas));
                    BackToSoulTriggered?.Invoke();
                    return false;
                }

                return true;
            });


        }));

        private ICommand _disconnectAction;
        public ICommand DisconnectAction =>
        _disconnectAction ?? (_disconnectAction = new Command(() =>
        {
            NetworkManager.Instance.DisconnectFromServer();
        }));*/
    }
}
