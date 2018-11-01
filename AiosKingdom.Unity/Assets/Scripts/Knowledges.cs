using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knowledges : MonoBehaviour
{
    public NetworkManager Network;

    public GameObject Content;
    public GameObject KnowledgeListItem;

    // Use this for initialization
    void Start()
    {
        Network.AskKnowledges();
    }

    public void LoadKnowledges()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var knowledge in DatasManager.Instance.Knowledges)
        {
            var skill = DatasManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(knowledge.BookId));

            var knowObj = Instantiate(KnowledgeListItem, Content.transform);
            var script = knowObj.GetComponent<KnowledgeListItem>();
            script.SetDatas(skill, knowledge.Rank);
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));
    }
}
