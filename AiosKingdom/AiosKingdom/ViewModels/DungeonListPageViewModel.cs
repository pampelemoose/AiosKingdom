using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class DungeonListPageViewModel : BaseViewModel
    {
        public DungeonListPageViewModel(INavigation nav)
            : base(nav)
        {
            Title = "Dungeons";
        }

        public List<DataModels.Dungeons.Dungeon> Dungeons => DatasManager.Instance.Dungeons;

        public DataModels.Dungeons.Dungeon SelectedDungeon
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    _navigation.PushModalAsync(new Views.Dungeon.EnterDungeonPage(value));
                }

                value = null;
            }
        }
    }
}
