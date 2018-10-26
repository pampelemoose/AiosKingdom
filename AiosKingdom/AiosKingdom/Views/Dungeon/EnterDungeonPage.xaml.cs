using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AiosKingdom.Views.Dungeon
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EnterDungeonPage : ContentPage
	{
		public EnterDungeonPage(Network.Adventures.Dungeon dungeon)
		{
			InitializeComponent();

            BindingContext = new ViewModels.Dungeon.EnterDungeonPageViewModel(Navigation, dungeon);
		}
	}
}