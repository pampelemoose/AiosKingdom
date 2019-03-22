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

        InputController.This.AddCallback("Home", (direction) =>
        {
            if (gameObject.activeSelf)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    if (direction == SwipeDirection.Up && !MenuBox.activeSelf)
                    {
                        MenuBox.SetActive(true);
                    }
                    else if (direction == SwipeDirection.Down && MenuBox.activeSelf)
                    {
                        MenuBox.SetActive(false);
                    }
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
