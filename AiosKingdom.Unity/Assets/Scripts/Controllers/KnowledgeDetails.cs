using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgeDetails : PaginationBox
{
    [Space(10)]
    [Header("Knowledge Details")]
    public Text Name;
    public Text Quality;
    public Text Cooldown;
    public Text Manacost;
    public Text TalentPoints;
    public Text Description;
    public Button Close;

    private JsonObjects.Knowledge _knowledge;
    private List<JsonObjects.Skills.Talent> _talents;
    private List<JsonObjects.Skills.Inscription> _inscriptions;

    private JsonObjects.Skills.BuiltSkill _builtSkill;

    private void Awake()
    {
        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void ShowDetails(JsonObjects.Skills.Book book, JsonObjects.Knowledge knowledge)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        var talents = book.Talents.Where(t => knowledge.Talents.Any(k => k.TalentId.Equals(t.Id))).ToList();
        var talCooldown = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.Cooldown).Sum(t => t.Value);
        var talManacost = talents.Where(t => t.Type == JsonObjects.Skills.TalentType.ManaCost).Sum(t => t.Value);

        Name.text = book.Name;
        Quality.text = book.Quality.ToString();
        Cooldown.text = $"{book.Cooldown} (-{talCooldown})";
        Manacost.text = $"{book.ManaCost} (-{talManacost})";
        Description.text = book.Description;
        TalentPoints.text = $"{knowledge.TalentPoints}";

        _knowledge = knowledge;
        _talents = book.Talents;
        _inscriptions = book.Inscriptions;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _inscriptions.Count, ShowInscriptions);

        ShowInscriptions();
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
            var talents = _talents.Where(t => _knowledge.Talents.Any(k => k.TalentId.Equals(t.Id))).ToList();
            var inscObj = Instantiate(ListItemPrefab, List.transform);
            var script = inscObj.GetComponent<InscriptionListItem>();
            script.SetDatas(insc, talents);
        }

        _pagination.SetIndicator((_inscriptions.Count / ItemPerPage) + (_inscriptions.Count % ItemPerPage > 0 ? 1 : 0));
    }

    public void ShowBuiltDetails(JsonObjects.Skills.BuiltSkill skill)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = skill.Name;
        Description.text = skill.Description;
        Quality.text = skill.Quality.ToString();
        Cooldown.text = $"{skill.Cooldown}";
        Manacost.text = $"{skill.ManaCost}";
        TalentPoints.text = "";

        _builtSkill = skill;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _builtSkill.Inscriptions.Count, ShowBuiltInscriptions);

        ShowBuiltInscriptions();
    }

    private void ShowBuiltInscriptions()
    {
        var inscriptions = _builtSkill.Inscriptions.OrderBy(i => i.Type).OrderByDescending(i => i.BaseMinValue).Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var insc in inscriptions)
        {
            var inscObj = Instantiate(ListItemPrefab, List.transform);
            var script = inscObj.GetComponent<InscriptionListItem>();
            script.SetBuiltDatas(insc);
        }

        _pagination.SetIndicator((_builtSkill.Inscriptions.Count / ItemPerPage) + (_builtSkill.Inscriptions.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
