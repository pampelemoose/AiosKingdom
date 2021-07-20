using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulListItem : MonoBehaviour
{
    public Text Name;
    public Text Level;

    public Slider ExpSlider;
    public Text ExpText;

    public Text Stamina;
    public Text Energy;
    public Text Strength;
    public Text Agility;
    public Text Intelligence;
    public Text Wisdom;

    public Button ConnectButton;

    public void SetDatas(JsonObjects.SoulInfos soul)
    {
        Name.text = soul.Name;
        Level.text = soul.Level.ToString();

        ExpSlider.maxValue = soul.TotalExperience;
        ExpSlider.value = soul.Experience;
        ExpText.text = $"{soul.Experience} / {soul.TotalExperience}";

        Stamina.text = $"{soul.Stamina}";
        Energy.text = $"{soul.Energy}";
        Strength.text = $"{soul.Strength}";
        Agility.text = $"{soul.Agility}";
        Intelligence.text = $"{soul.Intelligence}";
        Wisdom.text = $"{soul.Wisdom}";

        ConnectButton.onClick.RemoveAllListeners();
        ConnectButton.onClick.AddListener(() =>
        {
            UIManager.This.ShowLoading();

            NetworkManager.This.ConnectSoul(soul.Id);
        });
    }
}
