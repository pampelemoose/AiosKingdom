using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public NetworkManager Network;

    [Header("TopBar")]
    public Text Level;
    public Text Name;
    public Slider Experience;
    public Button ShowSummaryButton;

    [Space(10)]
    [Header("Summary")]
    public GameObject Summary;

    [Space(2)]
    [Header("General")]
    public Text Armor;
    public Text ItemLevel;

    [Space(2)]
    [Header("Attributes")]
    public Text Stamina;
    public Text Energy;
    public Text Strength;
    public Text Agility;
    public Text Intelligence;
    public Text Wisdom;

    [Space(2)]
    [Header("Currencies")]
    public Text Spirits;
    public Text Embers;
    public Text Bits;
    public Text Shards;

    void Start()
    {
        Network.AskSoulCurrentDatas();
        Network.AskCurrencies();
        Network.AskEquipment();
        Network.AskKnowledges();

        ShowSummaryButton.onClick.AddListener(() =>
        {
            Summary.SetActive(!Summary.activeSelf);
        });
    }

    void Update()
    {

    }

    public void UpdatePlayerDatas()
    {
        Level.text = DatasManager.Instance.Datas.Level.ToString();
        Name.text = DatasManager.Instance.Datas.Name;

        Experience.maxValue = DatasManager.Instance.Datas.RequiredExperience;
        Experience.value = DatasManager.Instance.Datas.CurrentExperience;

        Armor.text = DatasManager.Instance.Datas.Armor.ToString();
        ItemLevel.text = DatasManager.Instance.Datas.ItemLevel.ToString();

        Stamina.text = DatasManager.Instance.Datas.TotalStamina.ToString();
        Energy.text = DatasManager.Instance.Datas.TotalEnergy.ToString();
        Strength.text = DatasManager.Instance.Datas.TotalStrength.ToString();
        Agility.text = DatasManager.Instance.Datas.TotalAgility.ToString();
        Intelligence.text = DatasManager.Instance.Datas.TotalIntelligence.ToString();
        Wisdom.text = DatasManager.Instance.Datas.TotalWisdom.ToString();
    }

    public void UpdateCurrencies()
    {
        Spirits.text = DatasManager.Instance.Currencies.Spirits.ToString();
        Embers.text = DatasManager.Instance.Currencies.Embers.ToString();
        Bits.text = DatasManager.Instance.Currencies.Bits.ToString();
        Shards.text = DatasManager.Instance.Currencies.Shards.ToString();
    }
}
