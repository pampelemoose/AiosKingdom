using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BookDetails : PaginationBox
{
    [Space(10)]
    [Header("Book Details")]
    public Text Name;
    public Text Quality;
    public Text Cooldown;
    public Text Manacost;
    public Text Description;

    public GameObject PriceBox;
    public Text EmberPrice;
    public Button BuyButton;
    public Button Close;

    private List<JsonObjects.Skills.Inscription> _inscriptions;

    public void SetDatas(JsonObjects.Skills.Book book)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = book.Name;
        Quality.text = book.Quality.ToString();
        Cooldown.text = $"{book.Cooldown}";
        Manacost.text = $"{book.ManaCost}";
        Description.text = book.Description;
        EmberPrice.text = $"{book.EmberCost}";
        BuyButton.gameObject.SetActive(book.EmberCost <= DatasManager.Instance.Currencies.Embers);

        _inscriptions = book.Inscriptions;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
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

        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        PriceBox.SetActive(DatasManager.Instance.Knowledges.FirstOrDefault(k => k.BookId.Equals(book.Id)) == null);
    }

    private void ShowInscriptions()
    {
        var inscriptions = _inscriptions.OrderBy(i => i.Type).Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var insc in inscriptions)
        {
            var inscObj = Instantiate(ListItemPrefab, List.transform);
            var script = inscObj.GetComponent<InscriptionListItem>();
            script.SetDatas(insc, new List<JsonObjects.Skills.Talent>());
        }

        _pagination.SetIndicator((_inscriptions.Count / ItemPerPage) + (_inscriptions.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
