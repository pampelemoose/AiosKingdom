using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookDetails : MonoBehaviour
{
    public Text Name;
    public Text Quality;
    public Text Description;
    public GameObject Inscriptions;
    public GameObject InscriptionItem;

    public GameObject BuyBox;
    public Text EmberPrice;
    public Button BuyButton;

    private JsonObjects.Skills.Book _currentBook;

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        _currentBook = book;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = book.Name;
        Quality.text = book.Quality.ToString();

        ShowInscriptions();

        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(() =>
        {
            NetworkManager.This.LearnSkill(_currentBook.Id);
            gameObject.SetActive(false);
        });

        BuyBox.SetActive(DatasManager.Instance.Knowledges.FirstOrDefault(k => k.BookId.Equals(_currentBook.Id)) == null);
    }

    private void ShowInscriptions()
    {
        var inscriptions = _currentBook.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseValue).Skip(0).Take(8);

        Description.text = _currentBook.Description;
        EmberPrice.text = _currentBook.EmberCost.ToString();
        BuyButton.gameObject.SetActive(_currentBook.EmberCost <= DatasManager.Instance.Currencies.Embers);

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
