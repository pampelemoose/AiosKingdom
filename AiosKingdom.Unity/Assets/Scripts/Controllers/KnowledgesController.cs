using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KnowledgesController : PaginationBox
{
    [Space(10)]
    [Header("Knowledges")]
    public Button Talents;
    public TalentController TalentsPanel;
    //public Talents TalentsPanel;
    public Button Close;

    public KnowledgeDetails KnowledgeDetails;

    private List<JsonObjects.Knowledge> _knowledges;

    void Awake()
    {
        Talents.onClick.RemoveAllListeners();
        Talents.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            //TalentsPanel.ShowTalents();
            TalentsPanel.ShowTalents();
        });

        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            UIManager.This.ShowMain();
        });

        NetworkManager.This.AskKnowledges();
    }

    public void LoadKnowledges()
    {
        _knowledges = DatasManager.Instance.Knowledges;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _knowledges.Count, SetKnowledges);

        SetKnowledges();
    }

    private void SetKnowledges()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var knowledges = _knowledges.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var knowledge in knowledges)
        {
            var skill = DatasManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(knowledge.BookId));

            var knowObj = Instantiate(ListItemPrefab, List.transform);
            var script = knowObj.GetComponent<KnowledgeListItem>();

            script.SetDatas(skill, knowledge.TalentPoints);

            script.ShowDetailsButton.onClick.AddListener(() =>
            {
                KnowledgeDetails.ShowDetails(skill, knowledge);
            });
        }

        _pagination.SetIndicator((_knowledges.Count / ItemPerPage) + (_knowledges.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
