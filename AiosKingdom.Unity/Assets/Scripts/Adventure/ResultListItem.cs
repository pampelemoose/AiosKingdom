using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultListItem : MonoBehaviour
{
    public Text Content;
    public Text Value;

    public void SetDatas(JsonObjects.AdventureState.ActionResult result)
    {
        Content.text = string.Format("{0}", result.ResultType);
        Value.text = string.Format("{0}", result.Amount);
    }
}
