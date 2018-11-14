﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knowledges : MonoBehaviour
{
    public GameObject Content;
    public GameObject KnowledgeListItem;

    [Space(10)]
    [Header("Knowledge Details")]
    public GameObject KnowledgeDetails;

    // Use this for initialization
    void Start()
    {
        NetworkManager.This.AskKnowledges();
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

            script.ShowDetailsButton.onClick.AddListener(() =>
            {
                KnowledgeDetails.GetComponent<KnowledgeDetails>().SetDatas(skill, knowledge.Rank);
            });
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));
    }
}
