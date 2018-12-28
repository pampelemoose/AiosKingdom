using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeDetails : MonoBehaviour
{
    public Button CloseButton;
    public Text Name;
    public Text Quality;
    public Text Rank;
    public Text Description;
    public GameObject Inscriptions;
    public GameObject InscriptionItem;
    public GameObject InscriptionUpgradableItem;

    public GameObject UpgradeBox;
    public Text EmberPrice;
    public Button UpgradeButton;

    private JsonObjects.Skills.Book _currentBook;
    private int _rank;

    void Start()
    {
        CloseButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void ShowDetails(JsonObjects.Skills.Book book, int rank)
    {
        _currentBook = book;
        _rank = rank;

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = book.Name;
        Quality.text = book.Quality.ToString();
        Rank.text = rank.ToString();

        ShowPage(rank);

        UpgradeButton.onClick.RemoveAllListeners();
        UpgradeButton.onClick.AddListener(() =>
        {
            NetworkManager.This.LearnSkill(_currentBook.Id, _rank + 1);
            gameObject.SetActive(false);
        });

        UpgradeBox.SetActive(false);
    }

    private void ShowPage(int rank)
    {
        var inscriptions = _currentBook.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseValue).Skip(0).Take(8);

        UpgradeButton.gameObject.SetActive(false);

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
