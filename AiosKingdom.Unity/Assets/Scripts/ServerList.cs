using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerList : MonoBehaviour
{
    public GameObject List;
    public GameObject ServerListItem;

    public void SetServers(List<JsonObjects.GameServerInfos> servers)
    {
        foreach (var server in servers)
        {
            var serverItem = Instantiate(ServerListItem, List.transform);

            var script = serverItem.GetComponent<ServerListItem>();
            script.SetDatas(server);
        }

        StartCoroutine(UIHelper.SetScrollviewVerticalSize(List));
    }
}
