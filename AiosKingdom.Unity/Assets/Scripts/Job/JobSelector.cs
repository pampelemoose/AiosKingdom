using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobSelector : MonoBehaviour
{
    public GameObject JobSelection;
    public JsonObjects.JobType Job;

    void Awake()
    {
        var button = this.GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();
            JobSelection.GetComponent<Page>().CloseAction();
            NetworkManager.This.LearnJob(Job);
        });
    }
}
