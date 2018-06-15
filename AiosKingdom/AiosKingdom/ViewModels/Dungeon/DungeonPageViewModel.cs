using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels.Dungeon
{
    public class DungeonPageViewModel : BaseViewModel
    {
        public DungeonPageViewModel(INavigation nav) 
            : base(nav)
        {
        }

        public Network.AdventureState Room => DatasManager.Instance.Adventure;
        public Network.SoulDatas Datas => DatasManager.Instance.Datas;

        private KeyValuePair<Guid, Network.AdventureState.EnemyState>? _selectedEnemy;
        public KeyValuePair<Guid, Network.AdventureState.EnemyState>? SelectedEnemy
        {
            get { return _selectedEnemy; }
            set
            {
                _selectedEnemy = value;
                _skillsAction?.ChangeCanExecute();
                _consumablesAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private Command _skillsAction;
        public ICommand SkillsAction =>
            _skillsAction ?? (_skillsAction = new Command(() =>
            {
                //_navigation.PushModalAsync(new Views.Dungeon.ExitDungeonPage());
            }, () => { return _selectedEnemy != null; }));

        private Command _consumablesAction;
        public ICommand ConsumablesAction =>
            _consumablesAction ?? (_consumablesAction = new Command(() =>
            {
                //_navigation.PushModalAsync(new Views.Dungeon.ExitDungeonPage());
            }, () => { return _selectedEnemy != null; }));

        private ICommand _exitDungeonAction;
        public ICommand ExitDungeonAction =>
            _exitDungeonAction ?? (_exitDungeonAction = new Command(() =>
            {
                _navigation.PushModalAsync(new Views.Dungeon.ExitDungeonPage());
            }));
    }
}
