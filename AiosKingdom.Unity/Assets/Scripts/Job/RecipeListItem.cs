using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeListItem : MonoBehaviour
{
    public Text Name;
    public Text Price;
    public Button Action;

    public void SetDatas(JsonObjects.Recipe recipe)
    {
        Name.text = string.Format("{0}", recipe.Name);
        Price.text = string.Format("{0}", recipe.Price);
    }
}
