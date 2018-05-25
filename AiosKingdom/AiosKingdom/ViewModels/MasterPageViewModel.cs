using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class MasterPageViewModel : BaseViewModel
    {
        public MasterPageViewModel()
            : base(null)
        {
            _menus = new List<MasterPageItem>();

            _menus.Add(new MasterPageItem
            {
                TargetType = typeof(Views.DashboardPage),
                Title = "Dashboard"
            });
            _menus.Add(new MasterPageItem
            {
                TargetType = typeof(Views.KnowledgePage),
                Title = "Knowledge"
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
                TargetType = typeof(Views.MarketPage),
                Title = "Market"
            });
            _menus.Add(new MasterPageItem
            {
                TargetType = typeof(Views.BookstorePage),
                Title = "Bookstore"
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

        /*public event Action BackToSoulTriggered;
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
