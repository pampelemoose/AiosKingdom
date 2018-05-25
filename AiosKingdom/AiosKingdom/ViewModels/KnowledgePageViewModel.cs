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

            SetKnowledge();
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
            NetworkManager.Instance.LearnSkill(_selectedKnowledge.Knowledge.BookId, _selectedKnowledge.Page.Rank + 1);
        }, () =>
        {
            return (_selectedKnowledge != null && !_selectedKnowledge.IsMaxRank)
            && DatasManager.Instance.Soul?.Embers >= _selectedKnowledge?.CostToUpdate;
        }));

        private void SetKnowledge()
        {
            _knowledges = new List<Models.KnowledgeModel>();
            SelectedKnowledge = null;

            foreach (var slot in DatasManager.Instance.Soul.Knowledge.OrderBy(i => i.Rank).ToList())
            {
                var book = DatasManager.Instance.Books.FirstOrDefault(b => b.BookId.Equals(slot.BookId));
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
        }
    }
}
