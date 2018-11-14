using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
    public Image Image;
    public Text Name;
    public Text ServerSoulCount;
    public Text ServerSlotCount;
    public Button ConnectButton;

    public void SetDatas(JsonObjects.GameServerInfos infos)
    {
        Name.text = infos.Name;
        ServerSoulCount.text = infos.SoulCount.ToString();
        ServerSlotCount.text = (infos.SlotsLimit - infos.SlotsAvailable).ToString("0");

        ConnectButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();

            NetworkManager.This.AnnounceGameServerConnection(infos.Id);
        });
    }
}
