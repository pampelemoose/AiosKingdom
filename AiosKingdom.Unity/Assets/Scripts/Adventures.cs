using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Adventures : MonoBehaviour
{
    public GameObject Content;
    public GameObject AdventureListItem;

    void Start()
    {
        LoadAdventures();
    }

    private void LoadAdventures()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var adventure in DatasManager.Instance.Dungeons)
        {
            var advObj = Instantiate(AdventureListItem, Content.transform);

            var script = advObj.GetComponent<AdventureListItem>();
            script.SetDatas(adventure);
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));

        UIManager.This.HideLoading();
    }
}
