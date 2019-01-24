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
    public partial class SkillSelectionPage : ContentPage
    {
        public SkillSelectionPage(Network.AdventureState state)
        {
            InitializeComponent();

            BindingContext = new ViewModels.Dungeon.SkillSelectionPageViewModel(Navigation, state);
        }
    }
}