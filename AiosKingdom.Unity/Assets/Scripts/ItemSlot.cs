using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    private JsonObjects.Items.ItemType _type;
    private JsonObjects.Items.AItem _item;

    [Header("Main")]
    public Text Name;
    public Text Type;
    public Text Quality;

    [Space(10)]
    [Header("General Attributes")]
    public Text ItemLevel;
    public Text RequiredLevel;

    [Space(2)]
    [Header("Specific Attributes")]
    public Text SpecificLabel;
    public Text SpecificValue;
    public GameObject StatUI;
    public GameObject LeftStats;
    public GameObject RightStats;
    public GameObject Effects;
    public GameObject EffectUI;

    public void InitializeAsArmor(JsonObjects.Items.Armor item)
    {
        _type = JsonObjects.Items.ItemType.Armor;
        _item = item;

        InitAitem(_item);

        Type.text = item.Part.ToString();
        SpecificLabel.text = "Armor :";
        SpecificValue.text = item.ArmorValue.ToString();

        InitStats(item.Stats);
    }

    public void InitializeAsBag(JsonObjects.Items.Bag item)
    {
        _type = JsonObjects.Items.ItemType.Bag;
        _item = item;

        InitAitem(_item);

        Type.text = "Bag";
        SpecificLabel.text = "Slots :";
        SpecificValue.text = item.SlotCount.ToString();

        InitStats(item.Stats);
    }

    public void InitializeAsWeapon(JsonObjects.Items.Weapon item)
    {
        _type = JsonObjects.Items.ItemType.Weapon;
        _item = item;

        InitAitem(_item);

        Type.text = "Weapon";
        SpecificLabel.text = "Damages :";
        SpecificValue.text = item.MinDamages.ToString() + " - " + item.MaxDamages.ToString();

        InitStats(item.Stats);
    }

    public void InitializeAsConsumable(JsonObjects.Items.Consumable item)
    {
        _type = JsonObjects.Items.ItemType.Weapon;
        _item = item;

        InitAitem(_item);

        Type.text = "Consumable";
        SpecificLabel.gameObject.SetActive(false);
        SpecificValue.gameObject.SetActive(false);

        LeftStats.SetActive(false);
        RightStats.SetActive(false);
        Effects.SetActive(true);

        InitEffects(item.Effects);
    }

    private void InitAitem(JsonObjects.Items.AItem item)
    {
        Name.text = item.Name;
        Quality.text = item.Quality.ToString();

        ItemLevel.text = item.ItemLevel.ToString();
        RequiredLevel.text = item.UseLevelRequired.ToString();
    }

    private void InitStats(List<JsonObjects.Items.ItemStat> stats)
    {
        foreach (var stat in stats)
        {
            var panel = RightStats.transform.childCount > 2 ? LeftStats : RightStats;
            var statObj = Instantiate(StatUI, panel.transform);
            var script = statObj.GetComponent<StatUI>();

            script.Label.text = stat.Type.ToString();
            script.Value.text = stat.StatValue.ToString();
        }
    }

    private void InitEffects(List<JsonObjects.Items.ConsumableEffect> effects)
    {
        foreach (var effect in effects)
        {
            //var panel = RightStats.transform.childCount > 2 ? LeftStats : RightStats;
            //var statObj = Instantiate(StatUI, panel.transform);
            //var script = statObj.GetComponent<StatUI>();

            //script.Label.text = stat.Type.ToString();
            //script.Value.text = stat.StatValue.ToString();
        }
    }
}
