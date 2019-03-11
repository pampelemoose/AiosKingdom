using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobDoor : MonoBehaviour
{
    public GameObject JobSelection;
    public GameObject Recipes;

    void Awake()
    {
        var button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            JobSelection.SetActive(true);

            UIManager.This.HideMenu();
        });

        if (DatasManager.Instance.Job.Value != null)
        {
            SetJob(DatasManager.Instance.Job.Value);
        }

        DatasManager.Instance.Job.Changed += (job) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                if (job != null)
                {
                    SetJob(job);
                }
            });
        };
    }

    private void SetJob(JsonObjects.Job job)
    {
        var button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Recipes.SetActive(true);
            Recipes.GetComponent<Recipes>().LoadRecipes(job);

            UIManager.This.HideMenu();
        });

        this.GetComponentInChildren<Text>().text = string.Format("<{0}>", job.Type);

        switch (job.Type)
        {
            case JsonObjects.JobType.Alchemistry:

                break;
        }
    }
}
