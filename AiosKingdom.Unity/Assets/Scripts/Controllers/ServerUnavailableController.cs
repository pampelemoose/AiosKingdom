using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerUnavailableController : MonoBehaviour 
{
    public Button Reconnect;

    void Start()
    {
        Reconnect.onClick.RemoveAllListeners();
        Reconnect.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            NetworkManager.This.Reconnect();
        });
    }
}
