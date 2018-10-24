using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
    public NetworkManager Network;

    public Image Image;
    public Text Name;
    public Text ServerSoulCount;
    public Text ServerUsedSlot;
    public Text ServerMaxSlot;
    public Button ConnectButton;

    public void SetDatas(JsonObjects.GameServerInfos infos)
    {
        Name.text = infos.Name;
        ServerSoulCount.text = infos.SoulCount.ToString();
        ServerUsedSlot.text = infos.SlotsAvailable.ToString();
        ServerMaxSlot.text = infos.SlotsLimit.ToString();

        ConnectButton.onClick.AddListener(() =>
        {
            LoadingScreen.Loading.Show();

            Network.AnnounceGameServerConnection(infos.Id);
        });
    }
}
