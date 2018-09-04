using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class BookstorePageViewModel : BaseViewModel
    {
        public BookstorePageViewModel()
            : base(null)
        {
            Title = "Bookstore";

            Application.Current.Properties["AiosKingdom_TutorialStep"] = 2;
            Application.Current.SavePropertiesAsync();
            MessagingCenter.Send(this, MessengerCodes.TutorialChanged);

            IsInfoVisible = false;
        }

        private void Subscribe_SkillLearned()
        {
            MessagingCenter.Subscribe<NetworkManager, string>(this, MessengerCodes.SkillLearned, (sender, message) =>
            {
                IsBusy = false;
                IsInfoVisible = true;
                ResultMessage = message;
                MessagingCenter.Unsubscribe<NetworkManager, string>(this, MessengerCodes.SkillLearned);
            });
        }

        public List<DataModels.Skills.Book> Books
        {
            get
            {
                bool isTutorial = (bool)Application.Current.Properties["AiosKingdom_IsNewCharacter"];
                if (isTutorial)
                    return DatasManager.Instance.Books.Where(b => b.Pages.Any(p => p.EmberCost == 1)).ToList();

                return DatasManager.Instance.Books;
            }
        }

        public bool BookIsSelected { get { return _selectedBook != null; } }

        private DataModels.Skills.Book _selectedBook;
        public DataModels.Skills.Book SelectedBook
        {
            get { return _selectedBook; }
            set
            {
                _selectedBook = value;
                SelectedPage = null;
                if (_selectedBook != null && _selectedBook.Pages != null)
                {
                    _selectedBook.Pages = _selectedBook.Pages.OrderBy(p => p.Rank).ToList();
                    SelectedPage = _selectedBook.Pages[0];
                }
                _buyBookAction?.ChangeCanExecute();
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(BookIsSelected));
            }
        }

        public bool PageIsSelected { get { return _selectedPage != null; } }

        private DataModels.Skills.Page _selectedPage;
        public DataModels.Skills.Page SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(PageIsSelected));
            }
        }

        private Command _buyBookAction;
        public ICommand BuyBookAction =>
        _buyBookAction ?? (_buyBookAction = new Command(() =>
        {
            Subscribe_SkillLearned();

            IsBusy = true;
            NetworkManager.Instance.LearnSkill(_selectedBook.BookId, 1);
        }, () =>
        {
            return DatasManager.Instance.Currencies?.Embers >= _selectedBook?.Pages[0].EmberCost;
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
    }
}
