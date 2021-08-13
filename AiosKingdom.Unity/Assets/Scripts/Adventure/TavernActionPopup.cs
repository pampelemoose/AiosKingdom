using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TavernActionPopup : MonoBehaviour
{
    public Text TavernNameText;
    public Button SleepButton;
    public Button EatButton;
    public Button CloseButton;

    public void Open(JsonObjects.Adventures.Tavern tavern)
    {
        TavernNameText.text = tavern.Name;

        SleepButton.onClick.RemoveAllListeners();
        SleepButton.onClick.AddListener(() =>
        {
            NetworkManager.This.RestInTavern(tavern.Id);
            WorldManager.This.SetCanMove(true);
            gameObject.SetActive(false);
        });

        EatButton.onClick.RemoveAllListeners();
        EatButton.onClick.AddListener(() =>
        {
            
        });

        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(() =>
        {
            WorldManager.This.SetCanMove(true);
            gameObject.SetActive(false);
        });

        WorldManager.This.SetCanMove(false);
        gameObject.SetActive(true);
    }
}
