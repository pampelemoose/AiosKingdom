using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeDetails : MonoBehaviour
{
    public Text Name;
    public Text Quality;
    public Text TalentPoints;
    public Text Description;
    public GameObject Inscriptions;
    public GameObject InscriptionItem;
    public GameObject InscriptionUpgradableItem;

    public void ShowDetails(JsonObjects.Skills.Book book, JsonObjects.Knowledge knowledge)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = book.Name;
        Description.text = book.Description;
        Quality.text = book.Quality.ToString();
        TalentPoints.text = knowledge.TalentPoints.ToString();

        ShowInscriptions(book);
    }

    private void ShowInscriptions(JsonObjects.Skills.Book book)
    {
        var inscriptions = book.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseValue).Skip(0).Take(8);

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

    public void ShowBuiltDetails(JsonObjects.Skills.BuiltSkill skill)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = skill.Name;
        Description.text = skill.Description;
        Quality.text = skill.Quality.ToString();
        TalentPoints.text = "";

        ShowBuiltInscriptions(skill);
    }

    private void ShowBuiltInscriptions(JsonObjects.Skills.BuiltSkill book)
    {
        var inscriptions = book.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseMinValue).Skip(0).Take(8);

        foreach (Transform child in Inscriptions.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var insc in inscriptions)
        {
            var inscObj = Instantiate(InscriptionItem, Inscriptions.transform);
            var script = inscObj.GetComponent<BookInscriptionItem>();
            script.SetBuiltDatas(insc);
        }
    }
}
