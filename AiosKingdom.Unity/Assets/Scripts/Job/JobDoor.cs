using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobDoor : MonoBehaviour, ICallbackHooker
{
    public GameObject JobSelection;
    public GameObject Recipes;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Job.Get, (message) =>
        {
            if (message.Success)
            {
                var job = JsonConvert.DeserializeObject<JsonObjects.Job>(message.Json);

                if (job != null)
                {
                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        _setJob(job);
                    });
                }
            }
            else
            {
                Debug.Log("Job Get error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Job.Learn, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                var job = JsonConvert.DeserializeObject<JsonObjects.Job>(message.Json);

                if (job != null)
                {
                    SceneLoom.Loom.QueueOnMainThread(() =>
                    {
                        _setJob(job);
                    });
                }
            }
            else
            {
                Debug.Log("Job Learn error : " + message.Json);
            }
        });
    }

    void Start()
    {
        var button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            JobSelection.SetActive(true);

            UIManager.This.HideMenu();
        });
    }

    private void _setJob(JsonObjects.Job job)
    {
        var button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            InputController.This.SetId("Recipes");
            Recipes.SetActive(true);
            Recipes.GetComponent<Recipes>().LoadRecipes(job);

            UIManager.This.HideMenu();
        });

        this.GetComponentInChildren<Text>().text = string.Format("<{0}>", job.Type);

        //switch (job.Type)
        //{
        //    case JsonObjects.JobType.Alchemistry:

        //        break;
        //}
    }
}
