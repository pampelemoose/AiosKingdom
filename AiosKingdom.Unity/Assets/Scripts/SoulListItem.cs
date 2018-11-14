using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulListItem : MonoBehaviour
{
    public Text Name;
    public Text Level;
    public Button ConnectButton;

    public void SetDatas(JsonObjects.SoulInfos soul)
    {
        Name.text = soul.Name;
        Level.text = soul.Level.ToString();

        ConnectButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();

            NetworkManager.This.ConnectSoul(soul.Id);
        });
    }
}
