using System;
using System.Collections.Generic;
using System.Linq;
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
            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.DungeonUpdated, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadingScreenManager.Instance.CloseLoadingScreen();

                    NotifyPropertyChanged(nameof(Room));
                    NotifyPropertyChanged(nameof(IsCleared));
                    SelectedEnemy = null;

                    if (IsCleared)
                    {
                        ShowLootsPanel = true;
                        LoadingScreenManager.Instance.AlertLoadingScreen("Room Cleaned", "You cleaned the room.");
                        //LoadingScreenManager.Instance.OpenLoadingScreen("Retrieving Loots, pelase wait...");
                        NetworkManager.Instance.LootDungeonRoom();
                    }
                });
            });

            MessagingCenter.Subscribe<NetworkManager, List<Network.LootItem>>(this, MessengerCodes.DungeonLootsReceived, (sender, loots) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Loots = loots;
                    LoadingScreenManager.Instance.CloseLoadingScreen();
                });
            });

            NetworkManager.Instance.UpdateDungeonRoom();
        }

        public Network.AdventureState Room => DatasManager.Instance.Adventure;
        public Network.SoulDatas Datas => DatasManager.Instance.Datas;

        private List<Network.LootItem> _loots;
        private List<Network.LootItem> Loots
        {
            get { return _loots; }
            set
            {
                _loots = value;
                NotifyPropertyChanged();
            }
        }

        private KeyValuePair<Guid, Network.AdventureState.EnemyState>? _selectedEnemy;
        public KeyValuePair<Guid, Network.AdventureState.EnemyState>? SelectedEnemy
        {
            get { return _selectedEnemy; }
            set
            {
                _selectedEnemy = value;
                _executeAction?.ChangeCanExecute();
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
                NotifyPropertyChanged(nameof(CanExecuteAction));
            }
        }

        private bool _isConsumableSelected;
        public bool IsConsumableSelected
        {
            get { return _isConsumableSelected; }
            set
            {
                _isConsumableSelected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CanExecuteAction));
            }
        }

        public bool CanExecuteAction => _isSkillSelected || _isConsumableSelected;

        private bool _showLootsPanel;
        public bool ShowLootsPanel
        {
            get { return _showLootsPanel; }
            set
            {
                _showLootsPanel = value;
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
                _executeAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private Models.Dungeon.ConsumableSelectionItemModel _selectedConsumable;
        public Models.Dungeon.ConsumableSelectionItemModel SelectedConsumable
        {
            get { return _selectedConsumable; }
            set
            {
                _selectedConsumable = value;
                _executeAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        public bool IsCleared
        {
            get
            {
                return Room.IsFightArea && Room.Enemies.Count == 0;
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
                IsSkillSelected = false;
                SelectedSkill = null;
            }

            if (IsConsumableSelected)
            {
                IsConsumableSelected = false;
                SelectedConsumable = null;
            }
        }

        private ICommand _skillsAction;
        public ICommand SkillsAction =>
            _skillsAction ?? (_skillsAction = new Command(() =>
            {
                MessagingCenter.Subscribe<SkillSelectionPageViewModel, Models.Dungeon.SkillSelectionItemModel>
                (this, MessengerCodes.DungeonSelectSkillEnded, (sender, skill) =>
                {
                    IsSkillSelected = true;
                    SelectedSkill = skill;
                    MessagingCenter.Unsubscribe<SkillSelectionPageViewModel, Models.Dungeon.SkillSelectionItemModel>
                    (this, MessengerCodes.DungeonSelectSkillEnded);
                });

                var page = new Views.Dungeon.SkillSelectionPage(Room);
                _navigation.PushModalAsync(page);
            }));

        private ICommand _consumablesAction;
        public ICommand ConsumablesAction =>
            _consumablesAction ?? (_consumablesAction = new Command(() =>
            {
                MessagingCenter.Subscribe<ConsumableSelectionPageViewModel, Models.Dungeon.ConsumableSelectionItemModel>
                (this, MessengerCodes.DungeonSelectConsumableEnded, (sender, consumable) =>
                {
                    IsConsumableSelected = true;
                    SelectedConsumable = consumable;
                    MessagingCenter.Unsubscribe<ConsumableSelectionPageViewModel, Models.Dungeon.ConsumableSelectionItemModel>
                    (this, MessengerCodes.DungeonSelectConsumableEnded);
                });

                var page = new Views.Dungeon.ConsumableSelectionPage();
                _navigation.PushModalAsync(page);
            }));

        private ICommand _exitDungeonAction;
        public ICommand ExitDungeonAction =>
            _exitDungeonAction ?? (_exitDungeonAction = new Command(() =>
            {
                _navigation.PushModalAsync(new Views.Dungeon.ExitDungeonPage());
            }));

        private Command _executeAction;
        public ICommand ExecuteAction =>
            _executeAction ?? (_executeAction = new Command(() =>
            {
                if (IsSkillSelected)
                {
                    LoadingScreenManager.Instance.OpenLoadingScreen($"Ending turn. Please wait.");
                    NetworkManager.Instance.DungeonUseSkill(_selectedSkill.KnowledgeId, 
                        (_selectedEnemy != null ? _selectedEnemy.Value.Key : Guid.Empty));
                }

                if (IsConsumableSelected)
                {
                    LoadingScreenManager.Instance.OpenLoadingScreen($"Ending turn. Please wait.");
                    NetworkManager.Instance.DungeonUseConsumable(_selectedConsumable.SlotId, 
                        (_selectedEnemy != null ? _selectedEnemy.Value.Key : Guid.Empty));
                }

                ResetNextMove();
            }, () => { return CanExecutCurrentAction(); }));

        private bool CanExecutCurrentAction()
        {
            if (IsSkillSelected)
            {
                var skillInscTypes = _selectedSkill.Skill.Inscriptions.Select(i => i.Type).ToList();

                if (skillInscTypes.Contains(DataModels.Skills.InscriptionType.Damages))
                {
                    return _selectedEnemy != null;
                }
            }

            if (IsConsumableSelected)
            {
                var consEffectTypes = _selectedConsumable.Consumable.Effects.Select(e => e.Type).ToList();

                // TODO : If there is any consumable type that can be used on enemies, need to add it there.
                return true;
            }

            return false;
        }

        private ICommand _leaveFinishedAction;
        public ICommand LeaveFinishedAction =>
            _leaveFinishedAction ?? (_leaveFinishedAction = new Command(() =>
            {
                LoadingScreenManager.Instance.OpenLoadingScreen($"Leaving room, you will receive {Room.StackedExperience} experience. Please wait.");
                NetworkManager.Instance.LeaveFinishedRoom();
            }));
        
    }
}
