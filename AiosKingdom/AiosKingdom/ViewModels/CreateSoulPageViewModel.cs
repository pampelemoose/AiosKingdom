using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class CreateSoulPageViewModel : BaseViewModel
    {
        public CreateSoulPageViewModel(INavigation nav) 
            : base(nav)
        {
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _createAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private Command _createAction;
        public ICommand CreateAction =>
            _createAction ?? (_createAction = new Command(() =>
            {
                ScreenManager.Instance.OpenLoadingScreen("Creating new soul...");
                NetworkManager.Instance.CreateSoul(_name);
            }, () => { return !string.IsNullOrWhiteSpace(_name) && _name.Length > 4; }));
    }
}
