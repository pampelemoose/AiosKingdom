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
	public partial class BuyItemPage : ContentPage
	{
		public BuyItemPage(ViewModels.ABuyItemViewModel viewModel)
		{
			InitializeComponent();

            BindingContext = viewModel;
		}
	}
}