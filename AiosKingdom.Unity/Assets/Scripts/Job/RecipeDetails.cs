using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDetails : MonoBehaviour, ICallbackHooker
{
    public Text Name;
    public Text Technique;
    public GameObject Combinaisons;
    public GameObject CombinaisonListItem;

    public void HookCallbacks()
    {
        InputController.This.AddCallback("RecipeDetails", (direction) =>
        {
            if (gameObject.activeSelf)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    if (direction == SwipeDirection.Down)
                    {
                        GetComponent<Page>().CloseAction();
                    }
                });
            }
        });
    }

    public void SetDatas(JsonObjects.Recipe recipe)
    {
        gameObject.SetActive(true);

        Name.text = string.Format(": {0}", recipe.Name);
        Technique.text = string.Format(": {0}", recipe.Technique);

        foreach (Transform child in Combinaisons.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var combinaison in recipe.Combinaisons)
        {
            var advObj = Instantiate(CombinaisonListItem, Combinaisons.transform);

            var script = advObj.GetComponent<CombinaisonListItem>();
            script.SetDatas(combinaison);
        }
    }
}
