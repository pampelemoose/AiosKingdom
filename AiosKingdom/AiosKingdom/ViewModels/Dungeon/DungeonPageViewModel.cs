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
                ResetNextMove();
                NotifyPropertyChanged();
            }
        }

        private bool _isSkillSelected;
        public bool IsSkillSelected
        {
            get { return _isSkillSelected; }
            set
            {
                _isSkillSelected = value;
                NotifyPropertyChanged();
            }
        }

        private Models.Dungeon.SkillSelectionItemModel _selectedSkill;
        public Models.Dungeon.SkillSelectionItemModel SelectedSkill
        {
            get { return _selectedSkill; }
            set
            {
                _selectedSkill = value;
                NotifyPropertyChanged();
            }
        }


        private ICommand _removeNextMoveAction;
        public ICommand RemoveNextMoveAction =>
            _removeNextMoveAction ?? (_removeNextMoveAction = new Command(() =>
            {
                ResetNextMove();
            }));
        
        private void ResetNextMove()
        {
            if (IsSkillSelected)
            {
                SelectedSkill = null;
                IsSkillSelected = false;
            }
        }

        private Command _skillsAction;
        public ICommand SkillsAction =>
            _skillsAction ?? (_skillsAction = new Command(() =>
            {
                MessagingCenter.Subscribe<SkillSelectionPageViewModel, Models.Dungeon.SkillSelectionItemModel>(this, MessengerCodes.DungeonSelectSkillEnded, (sender, skill) =>
                {
                    SelectedSkill = skill;
                    IsSkillSelected = true;
                    MessagingCenter.Unsubscribe<SkillSelectionPageViewModel, Models.Dungeon.SkillSelectionItemModel>(this, MessengerCodes.DungeonSelectSkillEnded);
                });

                var page = new Views.Dungeon.SkillSelectionPage(Room);
                _navigation.PushModalAsync(page);
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
