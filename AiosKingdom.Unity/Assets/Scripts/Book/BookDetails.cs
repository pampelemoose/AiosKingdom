using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookDetails : MonoBehaviour
{
    public Text Name;
    public Text Quality;
    public Text Cooldown;
    public Text Manacost;
    public Text Description;
    public GameObject Inscriptions;
    public GameObject InscriptionItem;

    public GameObject BuyBox;
    public Text EmberPrice;
    public Button BuyButton;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private List<JsonObjects.Skills.Inscription> _inscriptions;

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = string.Format(": {0}", book.Name);
        Quality.text = string.Format(": {0}", book.Quality);
        Cooldown.text = string.Format(": [{0}]", book.Cooldown);
        Manacost.text = string.Format(": [{0}]", book.ManaCost);
        Description.text = book.Description;
        EmberPrice.text = string.Format(": {0}", book.EmberCost);
        BuyButton.gameObject.SetActive(book.EmberCost <= DatasManager.Instance.Currencies.Embers);

        _inscriptions = book.Inscriptions;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _inscriptions.Count, ShowInscriptions);

        ShowInscriptions();

        BuyButton.onClick.RemoveAllListeners();
        BuyButton.onClick.AddListener(() =>
        {
            NetworkManager.This.LearnSkill(book.Id);
            gameObject.SetActive(false);
        });

        BuyBox.SetActive(DatasManager.Instance.Knowledges.FirstOrDefault(k => k.BookId.Equals(book.Id)) == null);
    }

    private void ShowInscriptions()
    {
        var inscriptions = _inscriptions.OrderBy(i => i.Type).Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (Transform child in Inscriptions.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var insc in inscriptions)
        {
            var inscObj = Instantiate(InscriptionItem, Inscriptions.transform);
            var script = inscObj.GetComponent<BookInscriptionItem>();
            script.SetDatas(insc, new List<JsonObjects.Skills.Talent>());
        }

        _pagination.SetIndicator((_inscriptions.Count / ItemPerPage) + (_inscriptions.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
