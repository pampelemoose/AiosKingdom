using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListItem : MonoBehaviour
{
    public Text Name;
    public Text ServerSoulCount;
    public Slider Slots;
    public Text SlotText;
    public Button ConnectButton;

    public void SetDatas(JsonObjects.GameServerInfos infos)
    {
        Name.text = infos.Name;
        ServerSoulCount.text = infos.SoulCount.ToString();
        Slots.maxValue = infos.SlotsLimit;
        Slots.value = infos.SlotsLimit - infos.SlotsAvailable;
        SlotText.text = $"{infos.SlotsLimit - infos.SlotsAvailable} / {infos.SlotsLimit}";

        ConnectButton.onClick.RemoveAllListeners();
        if (infos.Online)
        {
            ConnectButton.onClick.AddListener(() =>
            {
                UIManager.This.ShowLoading();

                NetworkManager.This.AnnounceGameServerConnection(infos.Id);
            });
        }
    }
}
