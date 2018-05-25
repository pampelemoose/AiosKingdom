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
        public BookstorePageViewModel(INavigation nav)
            : base(nav)
        {
            Title = "Bookstore";
        }

        public List<DataModels.Skills.Book> Books => DatasManager.Instance.Books;

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
            NetworkManager.Instance.LearnSkill(_selectedBook.BookId, 1);
        }, () =>
        {
            return DatasManager.Instance.Soul?.Embers >= _selectedBook?.Pages[0].EmberCost;
        }));
    }
}
