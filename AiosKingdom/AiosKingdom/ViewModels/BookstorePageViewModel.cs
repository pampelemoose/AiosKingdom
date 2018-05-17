using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                if (_selectedBook != null && _selectedBook.Pages != null)
                {
                    _selectedBook.Pages = _selectedBook.Pages.OrderBy(p => p.Rank).ToList();
                }
                SelectedPage = null;
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
    }
}
