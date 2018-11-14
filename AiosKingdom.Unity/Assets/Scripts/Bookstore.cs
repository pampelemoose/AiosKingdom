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

    private bool _isFilteredQuality = false;
    private JsonObjects.Skills.BookQuality _filterQuality;
    private bool _isFilteredTypes = false;
    private JsonObjects.Skills.InscriptionType _filterType;
    private bool _isFilteredStats = false;
    private JsonObjects.Stats _filterStat;

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
                BookDetails.GetComponent<BookDetails>().SetDatas(book);
            });
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));
    }
}
