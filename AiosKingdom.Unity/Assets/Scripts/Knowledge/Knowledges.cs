using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Knowledges : MonoBehaviour
{
    public Button Talents;
    public Talents TalentsPanel;

    public GameObject Content;
    public GameObject KnowledgeListItem;

    [Space(10)]
    [Header("Knowledge Details")]
    public KnowledgeDetails KnowledgeDetails;

    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private List<JsonObjects.Knowledge> _knowledges;

    void Awake()
    {
        Talents.onClick.RemoveAllListeners();
        Talents.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            TalentsPanel.ShowTalents();
        });

        NetworkManager.This.AskKnowledges();
    }

    public void LoadKnowledges()
    {
        _knowledges = DatasManager.Instance.Knowledges;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _knowledges.Count, SetKnowledges);

        SetKnowledges();
    }

    private void SetKnowledges()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var knowledges = _knowledges.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var knowledge in knowledges)
        {
            var skill = DatasManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(knowledge.BookId));

            var knowObj = Instantiate(KnowledgeListItem, Content.transform);
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
