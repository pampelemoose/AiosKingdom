using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class SpiritPillsPageViewModel : BaseViewModel
    {
        public SpiritPillsPageViewModel()
            : base(null)
        {
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetDatas();
                });
            });

            NetworkManager.Instance.AskCurrencies();

            Application.Current.Properties["AiosKingdom_TutorialStep"] = 2;
            Application.Current.SavePropertiesAsync();
            MessagingCenter.Send(this, MessengerCodes.TutorialChanged);

            IsInfoVisible = false;
        }

        private void Subscribe_UsePills()
        {
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = false;
                    SetDatas();
                });

                MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated);
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.LearnSpiritPills, (sender, msg) =>
            {
                IsBusy = false;
                IsInfoVisible = true;
                ResultMessage = msg;
                MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.LearnSpiritPills);
            });
        }

        private int _spirits;
        public int Spirits
        {
            get { return _spirits; }
            set
            {
                _spirits = value;
                NotifyPropertyChanged();
            }
        }

        private int _pillsAmount;
        public int PillsAmount
        {
            get { return _pillsAmount; }
            set
            {
                _pillsAmount = value;
                _decreasePillsAmountAction?.ChangeCanExecute();
                _increasePillsAmountAction?.ChangeCanExecute();
                _usePillsAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private Command _decreasePillsAmountAction;
        public ICommand DecreasePillAmountAction =>
        _decreasePillsAmountAction ?? (_decreasePillsAmountAction = new Command(() =>
        {
            --PillsAmount;
        }, () => { return _pillsAmount > 0; }));

        private Command _increasePillsAmountAction;
        public ICommand IncreasePillAmountAction =>
        _increasePillsAmountAction ?? (_increasePillsAmountAction = new Command(() =>
        {
            ++PillsAmount;
        }, () => { return _pillsAmount < Spirits; }));

        private Command _usePillsAction;
        public ICommand UsePillAction =>
        _usePillsAction ?? (_usePillsAction = new Command(() =>
        {
            Subscribe_UsePills();

            IsBusy = true;
            NetworkManager.Instance.UseSpiritPills(_statType, _pillsAmount);
        }, () => { return _pillsAmount > 0; }));

        private Network.Stats _statType = Network.Stats.Stamina;
        public Network.Stats StatType
        {
            get { return _statType; }
            set
            {
                _statType = value;
                NotifyPropertyChanged();
            }
        }

        private Command _staminaPillsAction;
        public ICommand StaminaPillsAction =>
        _staminaPillsAction ?? (_staminaPillsAction = new Command(() =>
        {
            StatType = Network.Stats.Stamina;
        }));

        private Command _energyPillsAction;
        public ICommand EnergyPillsAction =>
        _energyPillsAction ?? (_energyPillsAction = new Command(() =>
        {
            StatType = Network.Stats.Energy;
        }));

        private Command _strengthPillsAction;
        public ICommand StrengthPillsAction =>
        _strengthPillsAction ?? (_strengthPillsAction = new Command(() =>
        {
            StatType = Network.Stats.Strength;
        }));

        private Command _agilityPillsAction;
        public ICommand AgilityPillsAction =>
        _agilityPillsAction ?? (_agilityPillsAction = new Command(() =>
        {
            StatType = Network.Stats.Agility;
        }));

        private Command _intelligencePillsAction;
        public ICommand IntelligencePillsAction =>
        _intelligencePillsAction ?? (_intelligencePillsAction = new Command(() =>
        {
            StatType = Network.Stats.Intelligence;
        }));

        private Command _wisdomPillsAction;
        public ICommand WisdomPillsAction =>
        _wisdomPillsAction ?? (_wisdomPillsAction = new Command(() =>
        {
            StatType = Network.Stats.Wisdom;
        }));

        private bool _isInfoVisible;
        public bool IsInfoVisible
        {
            get { return _isInfoVisible; }
            set
            {
                _isInfoVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string _resultMessage;
        public string ResultMessage
        {
            get { return _resultMessage; }
            set
            {
                _resultMessage = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _closeInfoAction;
        public ICommand CloseInfoAction =>
            _closeInfoAction ?? (_closeInfoAction = new Command(() =>
            {
                IsInfoVisible = false;
            }));

        private void SetDatas()
        {
            Spirits = DatasManager.Instance.Currencies.Spirits;
            PillsAmount = 0;
        }
    }
}
