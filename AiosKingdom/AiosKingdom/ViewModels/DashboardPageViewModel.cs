using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class DashboardPageViewModel : BaseViewModel
    {
        public DashboardPageViewModel()
            : base(null)
        {
            Title = "Dashboard";
        }

        public DataModels.Soul Soul => DatasManager.Instance.Soul;
    }
}
