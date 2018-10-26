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
	public partial class CreateBagPage : ContentPage
	{
		public CreateBagPage(Network.Adventures.Dungeon dungeon)
		{
			InitializeComponent();

            BindingContext = new ViewModels.CreateBagPageViewModel(dungeon);
		}
	}
}