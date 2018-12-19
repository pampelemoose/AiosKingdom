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

        UpgradeBox.SetActive(_currentBook.Pages.FirstOrDefault(p => p.Rank == rank + 1) != null);
    }

    private void ShowPage(int rank)
    {
        var page = _currentBook.Pages.FirstOrDefault(p => p.Rank == rank);
        var nextPage = _currentBook.Pages.FirstOrDefault(p => p.Rank == rank + 1);
        var inscriptions = page.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseValue).Skip(0).Take(8);

        UpgradeButton.gameObject.SetActive(false);

        if (nextPage != null)
        {
            EmberPrice.text = nextPage.EmberCost.ToString();
            UpgradeButton.gameObject.SetActive(nextPage.EmberCost <= DatasManager.Instance.Currencies.Embers);
        }

        Description.text = page.Description;

        foreach (Transform child in Inscriptions.transform)
        {
            Destroy(child.gameObject);
        }

        if (nextPage == null)
        {
            foreach (var insc in inscriptions)
            {
                var inscObj = Instantiate(InscriptionItem, Inscriptions.transform);
                var script = inscObj.GetComponent<BookInscriptionItem>();
                script.SetDatas(insc);
            }
        }
        else
        {
            List<JsonObjects.Skills.Inscription> nextInscs = new List<JsonObjects.Skills.Inscription>();
            nextInscs.AddRange(nextPage.Inscriptions);
            foreach (var insc in inscriptions)
            {
                var nextInsc = nextInscs.FirstOrDefault(i => i.Type == insc.Type && i.StatType == insc.StatType);
                if (nextInsc == null)
                {
                    var inscObj = Instantiate(InscriptionItem, Inscriptions.transform);
                    var script = inscObj.GetComponent<BookInscriptionItem>();
                    script.SetDatas(insc);
                }
                else
                {
                    var inscObj = Instantiate(InscriptionUpgradableItem, Inscriptions.transform);
                    var script = inscObj.GetComponent<KnowledgeInscriptionUpgradableItem>();
                    script.SetDatas(insc, nextInsc);
                    nextInscs.Remove(nextInsc);
                }
            }

            foreach (var insc in nextInscs)
            {
                var inscObj = Instantiate(InscriptionUpgradableItem, Inscriptions.transform);
                var script = inscObj.GetComponent<KnowledgeInscriptionUpgradableItem>();
                script.SetDatas(new JsonObjects.Skills.Inscription(), insc);
            }
        }
    }
}
