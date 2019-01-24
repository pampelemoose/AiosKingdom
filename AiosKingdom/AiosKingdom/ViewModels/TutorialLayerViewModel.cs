using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class TutorialLayerViewModel : BaseViewModel
    {
        public TutorialLayerViewModel()
            : base(null)
        {
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.TutorialChanged, (sender) =>
            {
                TutorialChanged();
            });

            MessagingCenter.Subscribe<BookstorePageViewModel>(this, MessengerCodes.TutorialChanged, (sender) =>
            {
                TutorialChanged();
            });

            MessagingCenter.Subscribe<SpiritPillsPageViewModel>(this, MessengerCodes.TutorialChanged, (sender) =>
            {
                TutorialChanged();
            });

            MessagingCenter.Subscribe<HomePageViewModel>(this, MessengerCodes.TutorialChanged, (sender) =>
            {
                TutorialChanged();
            });
        }

        private void TutorialChanged()
        {
            NotifyPropertyChanged(nameof(IsVisible));
            NotifyPropertyChanged(nameof(IsStepOne));
            NotifyPropertyChanged(nameof(IsStepTwo));
            NotifyPropertyChanged(nameof(IsStepThree));
            NotifyPropertyChanged(nameof(IsStepFour));
            NotifyPropertyChanged(nameof(IsStepFive));
        }

        public bool IsVisible
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("AiosKingdom_IsNewCharacter"))
                {
                    bool isNew = (bool)Application.Current.Properties["AiosKingdom_IsNewCharacter"];

                    return isNew;
                }

                return true;
            }
        }

        public bool IsStepOne { get { return IsTutorialStep(1); } }
        public bool IsStepTwo { get { return IsTutorialStep(2); } }
        public bool IsStepThree { get { return IsTutorialStep(3); } }
        public bool IsStepFour { get { return IsTutorialStep(4); } }
        public bool IsStepFive { get { return IsTutorialStep(5); } }

        private ICommand _endTutorialAction;
        public ICommand EndTutorialAction =>
            _endTutorialAction ?? (_endTutorialAction = new Command(() =>
            {
                Application.Current.Properties["AiosKingdom_IsNewCharacter"] = false;
                Application.Current.SavePropertiesAsync();
                TutorialChanged();
            }));

        private bool IsTutorialStep(int step)
        {
            if (Application.Current.Properties.ContainsKey("AiosKingdom_TutorialStep"))
            {
                int currentStep = (int)Application.Current.Properties["AiosKingdom_TutorialStep"];

                return currentStep == step;
            }

            return false;
        }
    }
}
