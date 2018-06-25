using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels.Dungeon
{
    public class SkillSelectionPageViewModel : BaseViewModel
    {
        private Network.AdventureState _state;

        public SkillSelectionPageViewModel(INavigation nav, Network.AdventureState state)
            : base(nav)
        {
            _state = state;
        }

        public List<Models.Dungeon.SkillSelectionItemModel> Skills
        {
            get
            {
                var knowledges = DatasManager.Instance.Soul.Knowledge;
                var skills = new List<Models.Dungeon.SkillSelectionItemModel>();
                foreach (var knowledge in knowledges)
                {
                    var book = DatasManager.Instance.Books.FirstOrDefault(b => b.BookId.Equals(knowledge.BookId));

                    if (book != null)
                    {
                        var page = book.Pages.FirstOrDefault(p => p.Rank.Equals(knowledge.Rank));

                        if (page != null)
                        {
                            skills.Add(new Models.Dungeon.SkillSelectionItemModel
                            {
                                KnowledgeId = knowledge.Id,
                                BookName = book.Name,
                                Skill = page,
                                CanSelect = page.ManaCost <= _state.CurrentMana
                            });
                        }
                    }
                }

                return skills;
            }
        }

        private Models.Dungeon.SkillSelectionItemModel _selectedSkill;
        public Models.Dungeon.SkillSelectionItemModel SelectedSkill
        {
            get { return _selectedSkill; }
            set
            {
                _selectedSkill = value;
                _validateAction?.ChangeCanExecute();
                NotifyPropertyChanged();
            }
        }

        private ICommand _closeAction;
        public ICommand CloseAction =>
            _closeAction ?? (_closeAction = new Command(() =>
            {
                _navigation.PopModalAsync();
            }));

        private Command _validateAction;
        public ICommand ValidateAction =>
            _validateAction ?? (_validateAction = new Command(() =>
            {
                MessagingCenter.Send(this, MessengerCodes.DungeonSelectSkillEnded, _selectedSkill);
                _navigation.PopModalAsync();
            }, () => { return _selectedSkill == null ? false : _selectedSkill.CanSelect; }));
    }
}
