using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public HomePageViewModel()
            : base(null)
        {
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Currencies));
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.EquipmentUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Equipment));

                NotifyPropertyChanged(nameof(Head));
                NotifyPropertyChanged(nameof(Shoulder));
                NotifyPropertyChanged(nameof(Torso));
                NotifyPropertyChanged(nameof(Belt));
                NotifyPropertyChanged(nameof(Hand));
                NotifyPropertyChanged(nameof(Leg));
                NotifyPropertyChanged(nameof(Pants));
                NotifyPropertyChanged(nameof(Feet));

                NotifyPropertyChanged(nameof(Bag));

                NotifyPropertyChanged(nameof(WeaponRight));
                NotifyPropertyChanged(nameof(WeaponLeft));
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulDatasUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Datas));
            });

            NetworkManager.Instance.AskCurrencies();
            NetworkManager.Instance.AskEquipment();
            NetworkManager.Instance.AskSoulCurrentDatas();
            NetworkManager.Instance.AskKnowledges();
        }

        ~HomePageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.EquipmentUpdated);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.SoulDatasUpdated);
        }

        private bool _showSoulDetails;
        public bool ShowSoulDetails
        {
            get { return _showSoulDetails; }
            set
            {
                _showSoulDetails = value;
                NotifyPropertyChanged();
            }
        }

        private ContentView _homeContent;
        public ContentView HomeContent
        {
            get { return _homeContent; }
            set
            {
                _homeContent = value;
                NotifyPropertyChanged();
            }
        }

        private string _currentContent;
        private bool _showContent;
        public bool ShowContent
        {
            get { return _showContent; }
            set
            {
                _showContent = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _showSoulDetailsAction;
        public ICommand ShowSoulDetailsAction =>
            _showSoulDetailsAction ?? (_showSoulDetailsAction = new Command(() =>
            {
                ShowSoulDetails = !ShowSoulDetails;
            }));

        private ICommand _showContentAction;
        public ICommand ShowContentAction =>
            _showContentAction ?? (_showContentAction = new Command((content) =>
            {
                string contentName = (string)content;
                switch (contentName)
                {
                    case "Home":
                        ShowContent = false;
                        _currentContent = "";

                        Application.Current.Properties["AiosKingdom_TutorialStep"] = 5;
                        Application.Current.SavePropertiesAsync();
                        MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                        break;
                    case "Inventory":
                        ShowContent = !(_currentContent == "Inventory");
                        if (ShowContent)
                        {
                            _currentContent = "Inventory";
                            HomeContent = new Views.InventoryPage();
                        }
                        else
                            _currentContent = "";
                        break;
                    case "Knowledge":
                        ShowContent = !(_currentContent == "Knowledge");
                        if (ShowContent)
                        {
                            _currentContent = "Knowledge";
                            HomeContent = new Views.KnowledgePage();
                        }
                        else
                            _currentContent = "";
                        break;
                    case "Spirits":
                        ShowContent = !(_currentContent == "Spirits");
                        if (ShowContent)
                        {
                            _currentContent = "Spirits";
                            HomeContent = new Views.SpiritPillsPage();
                        }
                        else
                            _currentContent = "";
                        break;
                    case "Market":
                        ShowContent = !(_currentContent == "Market");
                        if (ShowContent)
                        {
                            _currentContent = "Market";
                            HomeContent = new Views.MarketPage();
                        }
                        else
                            _currentContent = "";
                        break;
                    case "Bookstore":
                        ShowContent = !(_currentContent == "Bookstore");
                        if (ShowContent)
                        {
                            _currentContent = "Bookstore";
                            HomeContent = new Views.BookstorePage();
                        }
                        else
                            _currentContent = "";
                        break;
                    case "Dungeons":
                        ShowContent = !(_currentContent == "Dungeons");
                        if (ShowContent)
                        {
                            _currentContent = "Dungeons";
                            HomeContent = new Views.DungeonListPage();
                        }
                        else
                            _currentContent = "";
                        break;
                }
            }));

        public Network.Currencies Currencies => DatasManager.Instance.Currencies;
        public Network.Equipment Equipment => DatasManager.Instance.Equipment;
        public Network.SoulDatas Datas => DatasManager.Instance.Datas;

        public Network.Items.Armor Head => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Head));
        public Network.Items.Armor Shoulder => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Shoulder));
        public Network.Items.Armor Torso => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Torso));
        public Network.Items.Armor Belt => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Belt));
        public Network.Items.Armor Hand => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Hand));
        public Network.Items.Armor Leg => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Leg));
        public Network.Items.Armor Pants => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Pants));
        public Network.Items.Armor Feet => DatasManager.Instance.Armors?.FirstOrDefault(a => a.Id.Equals(Equipment?.Feet));

        public Network.Items.Weapon WeaponRight => DatasManager.Instance.Weapons?.FirstOrDefault(a => a.Id.Equals(Equipment?.WeaponRight));
        public Network.Items.Weapon WeaponLeft => DatasManager.Instance.Weapons?.FirstOrDefault(a => a.Id.Equals(Equipment?.WeaponLeft));

        public Network.Items.Bag Bag => DatasManager.Instance.Bags?.FirstOrDefault(a => a.Id.Equals(Equipment?.Bag));

        private bool _showArmorDetails;
        public bool ShowArmorDetails
        {
            get { return _showArmorDetails; }
            set
            {
                _showArmorDetails = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showWeaponDetails;
        public bool ShowWeaponDetails
        {
            get { return _showWeaponDetails; }
            set
            {
                _showWeaponDetails = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showBagDetails;
        public bool ShowBagDetails
        {
            get { return _showBagDetails; }
            set
            {
                _showBagDetails = value;
                NotifyPropertyChanged();
            }
        }

        private Network.Items.AItem _selectedItem;
        public Network.Items.AItem SelectedItem => _selectedItem;

        private ICommand _showArmorAction;
        public ICommand ShowArmorAction =>
            _showArmorAction ?? (_showArmorAction = new Command((item) =>
            {
                var armor = (Network.Items.Armor)item;

                if (armor == null) return;

                if (_selectedItem == null || (_selectedItem != null && _selectedItem == armor) || ShowWeaponDetails || ShowBagDetails)
                {
                    ShowArmorDetails = !ShowArmorDetails;
                }

                if (ShowArmorDetails)
                {
                    ShowWeaponDetails = false;
                    ShowBagDetails = false;
                    _selectedItem = armor;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
                else
                {
                    _selectedItem = null;
                }
            }));

        private ICommand _showWeaponAction;
        public ICommand ShowWeaponAction =>
            _showWeaponAction ?? (_showWeaponAction = new Command((item) =>
            {
                var weapon = (Network.Items.Weapon)item;

                if (weapon == null) return;

                if (_selectedItem == null || (_selectedItem != null && _selectedItem == weapon) || ShowArmorDetails || ShowBagDetails)
                {
                    ShowWeaponDetails = !ShowWeaponDetails;
                }

                if (ShowWeaponDetails)
                {
                    ShowArmorDetails = false;
                    ShowBagDetails = false;
                    _selectedItem = weapon;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
                else
                {
                    _selectedItem = null;
                }
            }));

        private ICommand _showBagAction;
        public ICommand ShowBagAction =>
            _showBagAction ?? (_showBagAction = new Command((item) =>
            {
                var bag = (Network.Items.Bag)item;

                if (bag == null) return;

                if (_selectedItem == null || (_selectedItem != null && _selectedItem == bag) || ShowArmorDetails || ShowWeaponDetails)
                {
                    ShowBagDetails = !ShowBagDetails;
                }

                if (ShowBagDetails)
                {
                    ShowArmorDetails = false;
                    ShowWeaponDetails = false;
                    _selectedItem = bag;
                    NotifyPropertyChanged(nameof(SelectedItem));
                }
                else
                {
                    _selectedItem = null;
                }
            }));
    }
}
