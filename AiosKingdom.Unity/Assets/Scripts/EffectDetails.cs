using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectDetails : MonoBehaviour
{
    public Text Name;
    public Text Description;
    public Text Type;
    public Text EffectValue;
    public Text EffectTime;
    public Button Close;

    public void SetDatas(JsonObjects.Items.ConsumableEffect effect)
    {
        gameObject.SetActive(true);

        Name.text = effect.Name;
        Description.text = effect.Description;

        Type.text = effect.Type.ToString();

        EffectValue.text = string.Format("[{0}]", effect.AffectValue);
        EffectTime.text = string.Format("[{0}]", effect.AffectTime);

        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
