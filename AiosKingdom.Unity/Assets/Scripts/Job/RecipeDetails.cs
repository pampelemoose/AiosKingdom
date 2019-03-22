using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDetails : MonoBehaviour
{
    public Text Name;
    public Text Technique;

    public void SetDatas(JsonObjects.Recipe recipe)
    {
        gameObject.SetActive(true);

        Name.text = string.Format(": {0}", recipe.Name);
        Technique.text = string.Format(": {0}", recipe.Technique);
    }
}
