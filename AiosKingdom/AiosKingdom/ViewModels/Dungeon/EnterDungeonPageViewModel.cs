using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels.Dungeon
{
    public class EnterDungeonPageViewModel : BaseViewModel
    {
        private DataModels.Dungeons.Dungeon _dungeon;

        public EnterDungeonPageViewModel(INavigation nav, DataModels.Dungeons.Dungeon dungeon)
            : base(nav)
        {
            _dungeon = dungeon;
        }

        ~EnterDungeonPageViewModel()
        {
        }

        public DataModels.Dungeons.Dungeon Dungeon => _dungeon;

        public string Confirmation => $"Are you sure you want to enter {_dungeon.Name} ? Level {_dungeon.RequiredLevel} is required.";
        public string Warning => "**Each room you clear increase the amount of experience you receive and the chances of drop for items gets higher. Leaving before the end will reset those bonuses. The dungeon will stay open until cleared.**";

        private ICommand _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                _navigation.PopModalAsync();
            }));

        private ICommand _enterAction;
        public ICommand EnterAction =>
            _enterAction ?? (_enterAction = new Command(() =>
            {
                _navigation.PopModalAsync();

                IsBusy = true;
                NetworkManager.Instance.EnterDungeon(_dungeon.DungeonId);
            }));
    }
}
