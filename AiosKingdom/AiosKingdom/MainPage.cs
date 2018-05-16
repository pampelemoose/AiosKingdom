using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom
{
    public class MainPage : MasterDetailPage
    {
        private ViewModels.MasterPageViewModel _masterViewModel;

        public MainPage()
        {
            MasterBehavior = MasterBehavior.Popover;

            _masterViewModel = new ViewModels.MasterPageViewModel();
            //_masterViewModel.OnSoulList = true;
            _masterViewModel.PageChangeTriggered += (type) => {
                Detail = new NavigationPage((Page)Activator.CreateInstance(type));
                IsPresented = false;
                Title = type.ToString();
                //_masterViewModel.OnSoulList = false;
            };

            /*_masterViewModel.BackToSoulTriggered += () => {
                _masterViewModel.OnSoulList = true;
                IsPresented = false;
                Detail = new NavigationPage(new Views.SoulListPage());
            };

            DatasManager.Instance.CurrentSoulChanged += () =>
            {
                Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                    if (_masterViewModel.ConnectedToSoul)
                    {
                        _masterViewModel.OnSoulList = false;
                        Detail = new NavigationPage(new Views.DashboardPage());
                        return false;
                    }
                    return true;
                });
            };*/

            Master = new Views.MasterPage(_masterViewModel)
            {
                Title = "Aios Kingdom"
            };
            Detail = new NavigationPage(new Views.DashboardPage());
        }
    }
}
