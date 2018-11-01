using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerList : MonoBehaviour
{
    public NetworkManager Network;

    public GameObject List;
    public GameObject ServerListItem;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetServers(List<JsonObjects.GameServerInfos> servers)
    {
        foreach (var server in servers)
        {
            var serverItem = Instantiate(ServerListItem, List.transform);

            var script = serverItem.GetComponent<ServerListItem>();
            script.Network = Network;
            script.SetDatas(server);
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(List));
    }
}
