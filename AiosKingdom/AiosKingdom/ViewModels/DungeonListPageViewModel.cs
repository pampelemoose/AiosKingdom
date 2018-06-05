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

        private ICommand _enterDungeonAction;
        public ICommand EnterDungeonAction =>
            _enterDungeonAction ?? (_enterDungeonAction = new Command((row) =>
            {
                var dungeon = (DataModels.Dungeons.Dungeon)row;

                _navigation.PushModalAsync(new Views.Dungeon.EnterDungeonPage(dungeon));
            }));
    }
}
