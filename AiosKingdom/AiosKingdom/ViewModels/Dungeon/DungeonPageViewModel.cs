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
                ShowLootsPanel = false;
                NotifyPropertyChanged(nameof(Room));
                NotifyPropertyChanged(nameof(IsCleared));
                NotifyPropertyChanged(nameof(IsRestArea));

                ResetNextMove();

                if (IsCleared || Room.IsExit)
                {
                    IsEnemyTurn = false;
                    ShowLootsPanel = true;
                    IsBusy = true;
                    NetworkManager.Instance.GetDungeonRoomLoots();
                }
                else
                {
                    IsBusy = false;
                }
            });


            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Currencies));
            });

            MessagingCenter.Subscribe<NetworkManager, List<Network.LootItem>>(this, MessengerCodes.DungeonLootsReceived, (sender, loots) =>
            {
                IsBusy = false;
                Loots = loots;
            });

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.EnemyTurnEnded, (sender) =>
            {
                IsEnemyTurn = false;
                IsBusy = false;
            });

            NetworkManager.Instance.AskInventory();
            NetworkManager.Instance.AskKnowledges();
            NetworkManager.Instance.UpdateDungeonRoom();
        }

        ~DungeonPageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.DungeonUpdated);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.CurrenciesUpdated);
            MessagingCenter.Unsubscribe<NetworkManager, List<Network.LootItem>>(this, MessengerCodes.DungeonLootsReceived);
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.EnemyTurnEnded);
        }

        public Network.AdventureState Room => DatasManager.Instance.Adventure;
        public Network.SoulDatas Datas => DatasManager.Instance.Datas;
        public Network.Currencies Currencies => DatasManager.Instance.Currencies;

        private List<Network.LootItem> _loots;
        public List<Network.LootItem> Loots
        {
            get { return _loots; }
            set
            {
                _loots = value;
                NotifyPropertyChanged();
            }
        }

        public Network.LootItem SelectedLoot
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    IsBusy = true;
                    NetworkManager.Instance.LootDungeonItem(value.LootId);
                }
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

        private KeyValuePair<Guid, Network.AdventureState.ShopState>? _selectedShopItem;
        public KeyValuePair<Guid, Network.AdventureState.ShopState>? SelectedShopItem
        {
            get { return _selectedShopItem; }
            set
            {
                _selectedShopItem = value;
                _buyShopItemAction?.ChangeCanExecute();
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
                return Room.IsFightArea && Room.Enemies.Count == 0 && !Room.IsExit;
            }
        }

        public bool IsRestArea => Room.IsRestingArea;

        private bool _isEnemyTurn;
        public bool IsEnemyTurn
        {
            get { return _isEnemyTurn; }
            set
            {
                _isEnemyTurn = value;
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
            SelectedEnemy = null;

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

        private ICommand _enemyTurnAction;
        public ICommand EnemyTurnAction =>
            _enemyTurnAction ?? (_enemyTurnAction = new Command(() =>
            {
                IsBusy = true;
                NetworkManager.Instance.EnemyTurn();
            }));

        private Command _buyShopItemAction;
        public ICommand BuyShopItemAction =>
            _buyShopItemAction ?? (_buyShopItemAction = new Command(() =>
            {
                var page = new Views.BuyItemPage(new ShopBuyItemPageViewModel(_navigation,
                    (KeyValuePair<Guid, Network.AdventureState.ShopState>)_selectedShopItem));
                _navigation.PushModalAsync(page);
            }, () => { return _selectedShopItem != null && _selectedShopItem.Value.Value.ShardPrice <= DatasManager.Instance.Currencies.Shards; }));

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

        private ICommand _endTurnActionAction;
        public ICommand EndTurnAction =>
            _endTurnActionAction ?? (_endTurnActionAction = new Command(() =>
            {
                IsBusy = true;
                NetworkManager.Instance.DoNothingTurn();

                IsEnemyTurn = true;
                ResetNextMove();
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
                    IsBusy = true;
                    NetworkManager.Instance.DungeonUseSkill(_selectedSkill.KnowledgeId,
                        (_selectedEnemy != null ? _selectedEnemy.Value.Key : Guid.Empty));
                }

                if (IsConsumableSelected)
                {
                    IsBusy = true;
                    NetworkManager.Instance.DungeonUseConsumable(_selectedConsumable.SlotId,
                        (_selectedEnemy != null ? _selectedEnemy.Value.Key : Guid.Empty));
                }

                IsEnemyTurn = true;
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

                return true;
            }

            if (IsConsumableSelected)
            {
                var consEffectTypes = _selectedConsumable.Consumable.Effects.Select(e => e.Type).ToList();

                // TODO : If there is any consumable type that can be used on enemies, need to add it there.
                return true;
            }

            return false;
        }

        private ICommand _playerRestAction;
        public ICommand PlayerRestAction =>
            _playerRestAction ?? (_playerRestAction = new Command(() =>
            {
                IsBusy = true;
                NetworkManager.Instance.PlayerRest();
            }));

        private ICommand _nextRoomAction;
        public ICommand NextRoomAction =>
            _nextRoomAction ?? (_nextRoomAction = new Command(() =>
            {
                IsBusy = true;
                NetworkManager.Instance.OpenDungeonRoom();
            }));

        private ICommand _leaveFinishedAction;
        public ICommand LeaveFinishedAction =>
            _leaveFinishedAction ?? (_leaveFinishedAction = new Command(() =>
            {
                IsBusy = true;
                NetworkManager.Instance.LeaveFinishedRoom();
            }));
    }
}
