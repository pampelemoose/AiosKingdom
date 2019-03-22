using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Knowledges : MonoBehaviour, ICallbackHooker
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

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.Knowledges, (message) =>
        {
            if (message.Success)
            {
                var knowledges = JsonConvert.DeserializeObject<List<JsonObjects.Knowledge>>(message.Json);

                if (knowledges.Count == 0)
                {
                    //Application.Current.Properties["AiosKingdom_IsNewCharacter"] = true;
                    //Application.Current.Properties["AiosKingdom_TutorialStep"] = 1;
                    //Application.Current.SavePropertiesAsync();
                    //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                }

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _loadKnowledges(knowledges);
                });
            }
            else
            {
                Debug.Log("Knowledges error : " + message.Json);
            }
        });

        InputController.This.AddCallback("Knowledges", (direction) =>
        {
            if (gameObject.activeSelf)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    if (direction == SwipeDirection.Up)
                    {
                        InputController.This.SetId("Talents");
                        UIManager.This.ShowLoading();
                        TalentsPanel.ShowTalents();
                    }
                    if (direction == SwipeDirection.Down)
                        GetComponent<Page>().CloseAction();
                });
            }
        });
    }

    void Start()
    {
        Talents.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            TalentsPanel.ShowTalents();
        });
    }

    private void _loadKnowledges(List<JsonObjects.Knowledge> knowledges)
    {
        _knowledges = knowledges;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, _knowledges.Count, _setKnowledges);

        _setKnowledges();
    }

    private void _setKnowledges()
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
