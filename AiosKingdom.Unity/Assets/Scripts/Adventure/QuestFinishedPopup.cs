using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestFinishedPopup : MonoBehaviour
{
    public Text QuestName;
    public Text ObjectiveContent;
    public Button Close;

    public void Open(JsonObjects.Adventures.Quest quest)
    {
        QuestName.text = quest.Name;
        ObjectiveContent.text = quest.Description;

        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        gameObject.SetActive(true);
    }
}
