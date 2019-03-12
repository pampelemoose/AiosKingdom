using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Button Souls;
    public Button Quit;

    void Awake()
    {
        Souls.onClick.RemoveAllListeners();
        Souls.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            UIManager.This.ShowLoading();
            NetworkManager.This.DisconnectSoul();
            NetworkManager.This.DisconnectGame();
            NetworkManager.This.AskServerList();
        });

        Quit.onClick.RemoveAllListeners();
        Quit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
