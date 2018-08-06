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
            SetDatas();
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

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SpiritPillsFailed, (sender) =>
            {
                IsBusy = false;
                MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.SpiritPillsFailed);
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

        private DataModels.Soul.Stats _statType = DataModels.Soul.Stats.Stamina;
        public DataModels.Soul.Stats StatType
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
            StatType = DataModels.Soul.Stats.Stamina;
        }));

        private Command _energyPillsAction;
        public ICommand EnergyPillsAction =>
        _energyPillsAction ?? (_energyPillsAction = new Command(() =>
        {
            StatType = DataModels.Soul.Stats.Energy;
        }));

        private Command _strengthPillsAction;
        public ICommand StrengthPillsAction =>
        _strengthPillsAction ?? (_strengthPillsAction = new Command(() =>
        {
            StatType = DataModels.Soul.Stats.Strength;
        }));

        private Command _agilityPillsAction;
        public ICommand AgilityPillsAction =>
        _agilityPillsAction ?? (_agilityPillsAction = new Command(() =>
        {
            StatType = DataModels.Soul.Stats.Agility;
        }));

        private Command _intelligencePillsAction;
        public ICommand IntelligencePillsAction =>
        _intelligencePillsAction ?? (_intelligencePillsAction = new Command(() =>
        {
            StatType = DataModels.Soul.Stats.Intelligence;
        }));

        private Command _wisdomPillsAction;
        public ICommand WisdomPillsAction =>
        _wisdomPillsAction ?? (_wisdomPillsAction = new Command(() =>
        {
            StatType = DataModels.Soul.Stats.Wisdom;
        }));

        private void SetDatas()
        {
            PillsAmount = 0;
            Spirits = DatasManager.Instance.Currencies.Spirits;
        }
    }
}
