using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AiosKingdom.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MarketBuyItemPage : ContentPage
	{
		public MarketBuyItemPage(Models.MarketItemModel item, int quantity = 1)
		{
			InitializeComponent();

            BindingContext = new ViewModels.MarketBuyItemPageViewModel(Navigation, item, quantity);
		}
	}
}