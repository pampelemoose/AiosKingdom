using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookDetails : MonoBehaviour
{
    public Button CloseButton;
    public Text Name;
    public Text Quality;
    public Text Description;
    public Button PrevButton;
    public Button NextButton;
    public GameObject Inscriptions;
    public GameObject InscriptionItem;

    public GameObject BuyBox;
    public Text EmberPrice;
    public Button BuyButton;

    private JsonObjects.Skills.Book _currentBook;
    private int _currentPage = 0;

    void Start()
    {
        CloseButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        PrevButton.onClick.AddListener(() =>
        {
            ShowPage(_currentPage - 1);
        });

        NextButton.onClick.AddListener(() =>
        {
            ShowPage(_currentPage + 1);
        });
    }

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        _currentBook = book;
        _currentPage = 0;

        gameObject.SetActive(true);

        Name.text = book.Name;
        Quality.text = book.Quality.ToString();

        ShowPage(_currentPage);

        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(() =>
        {
            NetworkManager.This.LearnSkill(_currentBook.Id, 1);
            gameObject.SetActive(false);
        });

        BuyBox.SetActive(DatasManager.Instance.Knowledges.FirstOrDefault(k => k.BookId.Equals(_currentBook.Id)) == null);
    }

    private void ShowPage(int pageNumber)
    {
        _currentPage = pageNumber;
        var inscriptions = _currentBook.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseValue).Skip(0).Take(8);

        if (_currentPage == 0)
        {
            EmberPrice.text = _currentBook.EmberCost.ToString();
            BuyButton.gameObject.SetActive(_currentBook.EmberCost <= DatasManager.Instance.Currencies.Embers);
        }

        PrevButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(false);

        if (_currentPage > 0)
        {
            PrevButton.gameObject.SetActive(true);
        }

        Description.text = _currentBook.Description;

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
