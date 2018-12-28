using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogListItem : MonoBehaviour
{
    public Text Content;

    public void SetDatas(JsonObjects.AdventureState.ActionResult result)
    {
        Content.text = string.Format("{0} used {1} on {2} : {3} for {4}", 
            (result.FromId.Equals(Guid.Empty) ? "You" : DatasManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(result.FromId)).Name), 
            (result.IsConsumable ? DatasManager.Instance.Items.FirstOrDefault(m => m.Id.Equals(result.Id)).Name : DatasManager.Instance.Books.FirstOrDefault(m => m.Id.Equals(result.Id)).Name), 
            (result.ToId.Equals(Guid.Empty) ? "You" : DatasManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(result.ToId)).Name), 
            result.ResultType, result.Amount);
    }
}
