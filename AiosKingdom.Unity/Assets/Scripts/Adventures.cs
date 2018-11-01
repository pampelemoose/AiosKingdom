using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Adventures : MonoBehaviour
{
    public AiosKingdom Main;
    public NetworkManager Network;

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
            Destroy(child);
        }

        foreach (var adventure in DatasManager.Instance.Dungeons)
        {
            var advObj = Instantiate(AdventureListItem, Content.transform);

            var script = advObj.GetComponent<AdventureListItem>();
            script.Network = Network;
            script.SetDatas(adventure);
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(Content));

        LoadingScreen.Loading.Hide();
    }
}
