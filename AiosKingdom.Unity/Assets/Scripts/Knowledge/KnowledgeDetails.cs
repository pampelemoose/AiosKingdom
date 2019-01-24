using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeDetails : MonoBehaviour
{
    public Text Name;
    public Text Quality;
    public Text Cooldown;
    public Text Manacost;
    public Text TalentPoints;
    public Text Description;
    public GameObject Inscriptions;
    public GameObject InscriptionItem;

    [Space(10)]
    [Header("Pagination")]
    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private JsonObjects.Knowledge _knowledge;
    private List<JsonObjects.Skills.Talent> _talents;
    private List<JsonObjects.Skills.Inscription> _inscriptions;

    private JsonObjects.Skills.BuiltSkill _builtSkill;

    public void ShowDetails(JsonObjects.Skills.Book book, JsonObjects.Knowledge knowledge)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        var talents = book.Talents.Where(t => knowledge.Talents.Any(k => k.TalentId.Equals(t.Id))).ToList();
        var talCooldown = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.Cooldown).Sum(t => t.Value);
        var talManacost = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.ManaCost).Sum(t => t.Value);

        Name.text = string.Format(": {0}", book.Name);
        Description.text = book.Description;
        Quality.text = string.Format(": {0}", book.Quality);
        Cooldown.text = string.Format(": [{0}] (-{1})", book.Cooldown, talCooldown);
        Manacost.text = string.Format(": [{0}] (-{1})", book.ManaCost, talManacost);
        TalentPoints.text = string.Format(": [{0}]", knowledge.TalentPoints);

        _knowledge = knowledge;
        _talents = book.Talents;
        _inscriptions = book.Inscriptions;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _inscriptions.Count, ShowInscriptions);

        ShowInscriptions();
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
            var talents = _talents.Where(t => _knowledge.Talents.Any(k => k.TalentId.Equals(t.Id))).ToList();
            var inscObj = Instantiate(InscriptionItem, Inscriptions.transform);
            var script = inscObj.GetComponent<BookInscriptionItem>();
            script.SetDatas(insc, talents);
        }

        _pagination.SetIndicator((_inscriptions.Count / ItemPerPage) + (_inscriptions.Count % ItemPerPage > 0 ? 1 : 0));
    }

    public void ShowBuiltDetails(JsonObjects.Skills.BuiltSkill skill)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = string.Format(": {0}", skill.Name);
        Description.text = skill.Description;
        Quality.text = string.Format(": {0}", skill.Quality);
        Cooldown.text = string.Format(": [{0}]", skill.Cooldown);
        Manacost.text = string.Format(": [{0}]", skill.ManaCost);
        TalentPoints.text = "";

        _builtSkill = skill;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _builtSkill.Inscriptions.Count, ShowBuiltInscriptions);

        ShowBuiltInscriptions();
    }

    private void ShowBuiltInscriptions()
    {
        var inscriptions = _builtSkill.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseMinValue).Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

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

        _pagination.SetIndicator((_builtSkill.Inscriptions.Count / ItemPerPage) + (_builtSkill.Inscriptions.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
