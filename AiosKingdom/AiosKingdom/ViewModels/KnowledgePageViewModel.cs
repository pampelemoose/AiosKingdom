using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class KnowledgePageViewModel : BaseViewModel
    {
        public KnowledgePageViewModel()
            : base(null)
        {
            Title = "Knowledges";

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.KnowledgeUpdated, (sender) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetKnowledge();
                });
            });

            NetworkManager.Instance.AskKnowledges();

            IsInfoVisible = false;
        }

        ~KnowledgePageViewModel()
        {
            MessagingCenter.Unsubscribe<NetworkManager>(this, MessengerCodes.KnowledgeUpdated);
        }

        private void Subscribe_SkillLearned()
        {
            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SkillLearned, (sender, message) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = false;
                    IsInfoVisible = true;
                    ResultMessage = message;
                    MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.SkillLearned);
                });
            });
        }

        private List<Models.KnowledgeModel> _knowledges;
        public List<Models.KnowledgeModel> Knowledges => _knowledges;

        private Models.KnowledgeModel _selectedKnowledge;
        public Models.KnowledgeModel SelectedKnowledge
        {
            get { return _selectedKnowledge; }
            set
            {
                _selectedKnowledge = value;
                _knowledgeIsSelected = _selectedKnowledge != null;
                _upgradeSkillAction?.ChangeCanExecute();

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(KnowledgeIsSelected));
            }
        }

        private bool _knowledgeIsSelected;
        public bool KnowledgeIsSelected => _knowledgeIsSelected;

        private Command _upgradeSkillAction;
        public ICommand UpgradeSkillAction =>
        _upgradeSkillAction ?? (_upgradeSkillAction = new Command(() =>
        {
            Subscribe_SkillLearned();

            IsBusy = true;
            NetworkManager.Instance.LearnSkill(_selectedKnowledge.Knowledge.BookId, _selectedKnowledge.Page.Rank + 1);
        }, () =>
        {
            return (_selectedKnowledge != null && !_selectedKnowledge.IsMaxRank)
            && DatasManager.Instance.Currencies?.Embers >= _selectedKnowledge?.CostToUpdate;
        }));

        private bool _isInfoVisible;
        public bool IsInfoVisible
        {
            get { return _isInfoVisible; }
            set
            {
                _isInfoVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string _resultMessage;
        public string ResultMessage
        {
            get { return _resultMessage; }
            set
            {
                _resultMessage = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _closeInfoAction;
        public ICommand CloseInfoAction =>
            _closeInfoAction ?? (_closeInfoAction = new Command(() =>
            {
                IsInfoVisible = false;
            }));

        private void SetKnowledge()
        {
            _knowledges = new List<Models.KnowledgeModel>();
            SelectedKnowledge = null;

            foreach (var slot in DatasManager.Instance.Knowledges.OrderBy(i => i.Rank).ToList())
            {
                var book = DatasManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(slot.BookId));
                var isMaxRank = book.Pages.Exists(p => p.Rank.Equals(slot.Rank + 1));

                _knowledges.Add(new Models.KnowledgeModel
                {
                    Knowledge = slot,
                    Name = book.Name,
                    Quality = book.Quality,
                    Page = book.Pages.FirstOrDefault(p => p.Rank.Equals(slot.Rank)),
                    IsMaxRank = !isMaxRank,
                    CostToUpdate = isMaxRank ? book.Pages.FirstOrDefault(p => p.Rank.Equals(slot.Rank + 1)).EmberCost : 0
                });
            }

            NotifyPropertyChanged(nameof(Knowledges));
        }
    }
}
