using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookstoreListPopup : PaginationBox
{
    [Space(5)]
    [Header("Filters")]
    public Dropdown QualityDropdown;
    public Dropdown TypesDropdown;
    public Dropdown StatsDropdown;
    public Button Close;

    [Space(10)]
    [Header("Book Details")]
    public BookDetails BookDetails;

    private List<JsonObjects.Skills.Book> _books;

    private JsonObjects.Skills.BookQuality? _filterQuality;
    private JsonObjects.Skills.InscriptionType? _filterType;
    private JsonObjects.Stats? _filterStat;

    void Start()
    {
        QualityDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Skills.BookQuality)).ToList());
        TypesDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Skills.InscriptionType)).ToList());
        StatsDropdown.AddOptions(Enum.GetNames(typeof(JsonObjects.Stats)).ToList());

        QualityDropdown.onValueChanged.AddListener((value) =>
        {
            _filterQuality = null;
            if (value > 0)
            {
                _filterQuality = (JsonObjects.Skills.BookQuality)Enum.Parse(typeof(JsonObjects.Skills.BookQuality), QualityDropdown.options.ElementAt(value).text);
            }

            SetBooks();
        });

        TypesDropdown.onValueChanged.AddListener((value) =>
        {
            _filterType = null;

            if (value > 0)
            {
                _filterType = (JsonObjects.Skills.InscriptionType)Enum.Parse(typeof(JsonObjects.Skills.InscriptionType), TypesDropdown.options.ElementAt(value).text);
            }

            SetBooks();
        });

        StatsDropdown.onValueChanged.AddListener((value) =>
        {
            _filterType = null;
            if (value > 0)
            {
                _filterStat = (JsonObjects.Stats)Enum.Parse(typeof(JsonObjects.Stats), StatsDropdown.options.ElementAt(value).text);
            }

            SetBooks();
        });

        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Open(List<JsonObjects.Skills.Book> books)
    {
        LoadBooks(books);

        gameObject.SetActive(true);
    }

    public void LoadBooks(List<JsonObjects.Skills.Book> books)
    {
        _books = books;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _books.Count, SetBooks);

        SetBooks();
    }

    private void SetBooks()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var books = _books;

        if (_filterQuality != null)
        {
            books = books.Where(b => b.Quality == _filterQuality).ToList();
        }

        if (_filterType != null)
        {
            books = books.Where(b => (b.Inscriptions.Where(i => i.Type == _filterType)).Any()).ToList();
        }

        if (_filterStat != null)
        {
            books = books.Where(b => (b.Inscriptions.Where(i => i.StatType == _filterStat)).Any()).ToList();
        }

        _pagination.SetIndicator((books.Count / ItemPerPage) + (books.Count % ItemPerPage > 0 ? 1 : 0));

        books = books.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var book in books)
        {
            var bookObj = Instantiate(ListItemPrefab, List.transform);
            var script = bookObj.GetComponent<BookstoreListItem>();

            script.SetDatas(book);

            script.ShowDetailsButton.onClick.AddListener(() =>
            {
                BookDetails.SetDatas(book);
            });
        }
    }
}
