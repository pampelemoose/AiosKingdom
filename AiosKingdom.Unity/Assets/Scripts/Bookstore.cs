using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bookstore : MonoBehaviour
{
    public NetworkManager Network;

    public GameObject Content;
    public GameObject BookListItem;

    [Space(5)]
    [Header("Filters")]
    public Dropdown QualityDropdown;
    public Dropdown TypesDropdown;
    public Dropdown StatsDropdown;

    [Space(10)]
    [Header("Book Details")]
    public GameObject BookDetails;
    public Button CloseButton;
    public Text Name;
    public Text Quality;
    public Text Description;
    public Button PrevButton;
    public Button NextButton;
    public GameObject Inscriptions;
    public GameObject InscriptionItem;

    private bool _isFilteredQuality = false;
    private JsonObjects.Skills.BookQuality _filterQuality;
    private bool _isFilteredTypes = false;
    private JsonObjects.Skills.InscriptionType _filterType;
    private bool _isFilteredStats = false;
    private JsonObjects.Stats _filterStat;

    private JsonObjects.Skills.Book _currentBook;
    private int _currentPage = 0;

    void Start()
    {
        QualityDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Skills.BookQuality)).ToList());
        TypesDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Skills.InscriptionType)).ToList());
        StatsDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Stats)).ToList());

        QualityDropdown.onValueChanged.AddListener((value) =>
        {
            _isFilteredQuality = false;
            if (value > 0)
            {
                _isFilteredQuality = true;
                _filterQuality = (JsonObjects.Skills.BookQuality)Enum.Parse(typeof(JsonObjects.Skills.BookQuality), QualityDropdown.options.ElementAt(value).text);
            }

            LoadBooks();
        });

        TypesDropdown.onValueChanged.AddListener((value) =>
        {
            _isFilteredTypes = false;
            if (value > 0)
            {
                _isFilteredTypes = true;
                _filterType = (JsonObjects.Skills.InscriptionType)Enum.Parse(typeof(JsonObjects.Skills.InscriptionType), TypesDropdown.options.ElementAt(value).text);
            }

            LoadBooks();
        });

        StatsDropdown.onValueChanged.AddListener((value) =>
        {
            _isFilteredStats = false;
            if (value > 0)
            {
                _isFilteredStats = true;
                _filterStat = (JsonObjects.Stats)Enum.Parse(typeof(JsonObjects.Stats), StatsDropdown.options.ElementAt(value).text);
            }

            LoadBooks();
        });

        CloseButton.onClick.AddListener(() =>
        {
            BookDetails.SetActive(false);
        });

        PrevButton.onClick.AddListener(() =>
        {
            ShowPage(_currentPage - 1);
        });

        NextButton.onClick.AddListener(() =>
        {
            ShowPage(_currentPage + 1);
        });

        LoadBooks();
    }

    public void LoadBooks()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var books = DatasManager.Instance.Books;

        if (_isFilteredQuality)
        {
            books = books.Where(b => b.Quality == _filterQuality).ToList();
        }

        if (_isFilteredTypes)
        {
            books = books.Where(b => (b.Pages.SelectMany(p => p.Inscriptions).Where(i => i.Type == _filterType)).Any()).ToList();
        }
        
        if (_isFilteredStats)
        {
            books = books.Where(b => (b.Pages.SelectMany(p => p.Inscriptions).Where(i => i.StatType == _filterStat)).Any()).ToList();
        }

        foreach (var book in books)
        {
            var bookObj = Instantiate(BookListItem, Content.transform);
            var script = bookObj.GetComponent<BookListItem>();

            script.SetDatas(book);

            script.ShowDetailsButton.onClick.AddListener(() =>
            {
                ShowBookDetails(book);
            });
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));
    }

    private void ShowBookDetails(JsonObjects.Skills.Book book)
    {
        _currentBook = book;
        _currentPage = 0;

        BookDetails.SetActive(true);

        Name.text = book.Name;
        Quality.text = book.Quality.ToString();

        ShowPage(_currentPage);
    }

    private void ShowPage(int pageNumber)
    {
        _currentPage = pageNumber;
        var page = _currentBook.Pages.OrderBy(p => p.Rank).ElementAt(_currentPage);
        var inscriptions = page.Inscriptions.Skip(0).Take(4);

        PrevButton.interactable = false;
        NextButton.interactable = false;

        if (_currentPage > 0)
        {
            PrevButton.interactable = true;
        }

        if (_currentPage < _currentBook.Pages.Count - 1)
        {
            NextButton.interactable = true;
        }

        Description.text = page.Description;

        foreach (Transform child in Inscriptions.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var insc in inscriptions)
        {
            var inscObj = Instantiate(InscriptionItem, Inscriptions.transform);
            var script = inscObj.GetComponent<BookInscriptionItem>();
            script.SetDatas(insc);
        }
    }
}
