using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour, ICallbackHooker
{
    public Button Commands;
    public GameObject MenuBox;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Recipes, (message) =>
        {
            if (message.Success)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(true);
                });
            }
        });
    }

    void Start()
    {
        Commands.onClick.AddListener(() =>
        {
            MenuBox.SetActive(!MenuBox.activeSelf);
        });
    }

    public void Hide()
    {
        MenuBox.SetActive(false);
    }
}
