using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdventureFinishedPopup : MonoBehaviour
{
    public Text QuestName;
    public Button Close;

    public void Open(JsonObjects.Adventures.Adventure adventure)
    {
        QuestName.text = adventure.Name;

        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        gameObject.SetActive(true);
    }
}
