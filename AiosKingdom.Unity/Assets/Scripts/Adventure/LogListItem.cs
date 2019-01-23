using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogListItem : MonoBehaviour
{
    public Text From;
    public Text To;
    public Text Type;
    public Text Value;

    public void SetDatas(JsonObjects.AdventureState.ActionResult result)
    {
        var from = (result.FromId.Equals(Guid.Empty) ? "You" : DatasManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(result.FromId)).Name);
        var target = (result.ToId.Equals(Guid.Empty) ? "You" : DatasManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(result.ToId)).Name);

        //var action = (result.IsConsumable ? DatasManager.Instance.Items.FirstOrDefault(m => m.Id.Equals(result.Id)).Name :
        //        (result.FromId.Equals(Guid.Empty) ? DatasManager.Instance.Adventure.State.Skills.FirstOrDefault(m => m.Id.Equals(result.Id)).Name :
        //            DatasManager.Instance.Adventure.Enemies.FirstOrDefault(e => e.Value.MonsterId.Equals(result.FromId)).Value.State.Skills.FirstOrDefault(m => m.Inscriptions.Any(i => i.Id.Equals(result.Id))).Name));

        From.text = string.Format(": {0}", from);
        To.text = string.Format(": {0}", target);
        Type.text = string.Format(": {1} ({0})", result.Action, result.ResultType);
        Value.text = string.Format(": [{0}]", result.Amount);
    }
}
