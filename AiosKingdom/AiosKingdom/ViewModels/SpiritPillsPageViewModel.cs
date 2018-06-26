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
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.SoulUpdated, (sender) =>
            {
                SetDatas();
            });

            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SpiritPillUsed, (sender, message) =>
            {
                LoadingScreenManager.Instance.AlertLoadingScreen("Spirit Pills", message);
            });

            SetDatas();
        }

        ~SpiritPillsPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.SoulUpdated);
            MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.SpiritPillUsed);
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
                NotifyPropertyChanged();
            }
        }

        private Command _decreasePillsAmountAction;
        public ICommand DecreasePillAmountAction =>
        _decreasePillsAmountAction ?? (_decreasePillsAmountAction = new Command(() =>
        {
            --PillsAmount;
            _decreasePillsAmountAction?.ChangeCanExecute();
            _increasePillsAmountAction?.ChangeCanExecute();
            _usePillsAction.ChangeCanExecute();
        }, () => { return _pillsAmount > 0; }));

        private Command _increasePillsAmountAction;
        public ICommand IncreasePillAmountAction =>
        _increasePillsAmountAction ?? (_increasePillsAmountAction = new Command(() =>
        {
            ++PillsAmount;
            _decreasePillsAmountAction?.ChangeCanExecute();
            _increasePillsAmountAction?.ChangeCanExecute();
            _usePillsAction.ChangeCanExecute();
        }, () => { return _pillsAmount < Spirits; }));

        private Command _usePillsAction;
        public ICommand UsePillAction =>
        _usePillsAction ?? (_usePillsAction = new Command(() =>
        {
            LoadingScreenManager.Instance.OpenLoadingScreen("Using pills, please wait for the effect to apply..");
            NetworkManager.Instance.UseSpiritPills(_statType, _pillsAmount);

            _decreasePillsAmountAction?.ChangeCanExecute();
            _increasePillsAmountAction?.ChangeCanExecute();
            _usePillsAction.ChangeCanExecute();
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
            Spirits = DatasManager.Instance.Soul.Spirits;
        }
    }
}
