using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bookstore : MonoBehaviour
{
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

    private GameObject _bookDetails;
    private JsonObjects.Skills.BookQuality? _filterQuality;
    private JsonObjects.Skills.InscriptionType? _filterType;
    private JsonObjects.Stats? _filterStat;

    void Awake()
    {
        if (_bookDetails == null)
        {
            _bookDetails = Instantiate(BookDetails, transform);

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

                LoadBooks();
            });

            TypesDropdown.onValueChanged.AddListener((value) =>
            {
                _filterType = null;

                if (value > 0)
                {
                    _filterType = (JsonObjects.Skills.InscriptionType)Enum.Parse(typeof(JsonObjects.Skills.InscriptionType), TypesDropdown.options.ElementAt(value).text);
                }

                LoadBooks();
            });

            StatsDropdown.onValueChanged.AddListener((value) =>
            {
                _filterType = null;
                if (value > 0)
                {
                    _filterStat = (JsonObjects.Stats)Enum.Parse(typeof(JsonObjects.Stats), StatsDropdown.options.ElementAt(value).text);
                }

                LoadBooks();
            });
        }

        LoadBooks();
    }

    public void LoadBooks()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var books = DatasManager.Instance.Books;

        if (_filterQuality != null)
        {
            books = books.Where(b => b.Quality == _filterQuality).ToList();
        }

        if (_filterType != null)
        {
            books = books.Where(b => (b.Pages.SelectMany(p => p.Inscriptions).Where(i => i.Type == _filterType)).Any()).ToList();
        }

        if (_filterStat != null)
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
                _bookDetails.GetComponent<BookDetails>().SetDatas(book);
            });
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));
    }
}
