using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestObjectivePopup : MonoBehaviour
{
    public Text QuestName;
    public Text ObjectiveContent;
    public Button Close;

    public void Open(JsonObjects.Adventures.Quest quest, Guid objectiveId)
    {
        QuestName.text = quest.Name;

        var objective = quest.Objectives.FirstOrDefault(o => o.Id == objectiveId);
        if (objective != null)
        {
            ObjectiveContent.text = objective.Title;
        }

        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        gameObject.SetActive(true);
    }
}
