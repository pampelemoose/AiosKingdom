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

            _masterViewModel.PageChangeTriggered += (type) => {
                Detail = new NavigationPage((Page)Activator.CreateInstance(type));
                IsPresented = false;
                Title = type.ToString();
            };

            Master = new Views.MasterPage(_masterViewModel)
            {
                Title = "Aios Kingdom"
            };
            Detail = new NavigationPage(new Views.DashboardPage());
        }
    }
}
