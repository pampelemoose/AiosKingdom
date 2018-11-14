using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public GameObject Content;
    public GameObject ItemSlot;

    [Space(10)]
    [Header("Details")]
    public GameObject DetailPanel;
    public Text Name;
    public Text Type;
    public Text Quality;
    public Text ItemLevel;
    public Text RequiredLevel;
    public Text SpecialLabel;
    public Text SpecialValue;
    public GameObject Stats;
    public GameObject StatUI;

    public void UpdateEquipment()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var equipment = DatasManager.Instance.Equipment;
        if (!Guid.Empty.Equals(equipment.Head))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var head = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Head));
            script.InitializeAsArmor(head);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(head);
            });
        }
        if (!Guid.Empty.Equals(equipment.Shoulder))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var shoulder = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Shoulder));
            script.InitializeAsArmor(shoulder);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(shoulder);
            });
        }
        if (!Guid.Empty.Equals(equipment.Torso))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var torso = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Torso));
            script.InitializeAsArmor(torso);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(torso);
            });
        }
        if (!Guid.Empty.Equals(equipment.Belt))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var belt = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Belt));
            script.InitializeAsArmor(belt);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(belt);
            });
        }
        if (!Guid.Empty.Equals(equipment.Pants))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var pants = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Pants));
            script.InitializeAsArmor(pants);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(pants);
            });
        }
        if (!Guid.Empty.Equals(equipment.Leg))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var leg = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Leg));
            script.InitializeAsArmor(leg);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(leg);
            });
        }
        if (!Guid.Empty.Equals(equipment.Feet))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var feet = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Feet));
            script.InitializeAsArmor(feet);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(feet);
            });
        }
        if (!Guid.Empty.Equals(equipment.Hand))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var hand = DatasManager.Instance.Armors.FirstOrDefault(b => b.Id.Equals(equipment.Hand));
            script.InitializeAsArmor(hand);
            script.Action.onClick.AddListener(() =>
            {
                ShowArmorDetails(hand);
            });
        }
        if (!Guid.Empty.Equals(equipment.WeaponLeft))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var leftHand = DatasManager.Instance.Weapons.FirstOrDefault(b => b.Id.Equals(equipment.WeaponLeft));
            script.InitializeAsWeapon(leftHand);
            script.Action.onClick.AddListener(() =>
            {
                ShowWeaponsDetails(leftHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.WeaponRight))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<LightItemSlot>();
            var rightHand = DatasManager.Instance.Weapons.FirstOrDefault(b => b.Id.Equals(equipment.WeaponRight));
            script.InitializeAsWeapon(rightHand);
            script.Action.onClick.AddListener(() =>
            {
                ShowWeaponsDetails(rightHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.Bag))
        {
            var bagObj = Instantiate(ItemSlot, Content.transform);
            var script = bagObj.GetComponent<LightItemSlot>();
            var bag = DatasManager.Instance.Bags.FirstOrDefault(b => b.Id.Equals(equipment.Bag));
            script.InitializeAsBag(bag);
            script.Action.onClick.AddListener(() =>
            {
                ShowBagDetails(bag);
            });
        }
    }

    private void ShowArmorDetails(JsonObjects.Items.Armor armor)
    {
        InitAitem(armor);

        SpecialLabel.text = " * Armor        :";
        SpecialValue.text = string.Format("[{0}]", armor.ArmorValue);

        Type.text = string.Format("Armor - {0}", armor.Part);

        InitStats(armor.Stats);
    }

    private void ShowWeaponsDetails(JsonObjects.Items.Weapon weapon)
    {
        InitAitem(weapon);

        SpecialLabel.text = " * Damages      :";
        SpecialValue.text = string.Format("[{0} - {1}]", weapon.MinDamages, weapon.MaxDamages);

        Type.text = string.Format("{0} - {1}", weapon.HandlingType, weapon.WeaponType);

        InitStats(weapon.Stats);
    }

    private void ShowBagDetails(JsonObjects.Items.Bag bag)
    {
        InitAitem(bag);

        SpecialLabel.text = " * Slots        :";
        SpecialValue.text = string.Format("[{0}]", bag.SlotCount);

        Type.text = "Bag";

        InitStats(bag.Stats);
    }

    private void InitAitem(JsonObjects.Items.AItem item)
    {
        DetailPanel.SetActive(true);

        Name.text = item.Name;
        Quality.text = item.Quality.ToString();

        ItemLevel.text = string.Format("[{0}]", item.ItemLevel);
        RequiredLevel.text = string.Format("[{0}]", item.UseLevelRequired);
    }

    private static Dictionary<JsonObjects.Stats, string> _statSpaces = new Dictionary<JsonObjects.Stats, string>
    {
        { JsonObjects.Stats.Stamina, "      " },
        { JsonObjects.Stats.Energy, "       " },
        { JsonObjects.Stats.Strength, "     " },
        { JsonObjects.Stats.Agility, "      " },
        { JsonObjects.Stats.Intelligence, " " },
        { JsonObjects.Stats.Wisdom, "       " }
    };
    private static Dictionary<JsonObjects.Stats, Color> _statColors = new Dictionary<JsonObjects.Stats, Color>
    {
        { JsonObjects.Stats.Stamina, new Color(1, 0, 0) },
        { JsonObjects.Stats.Energy, new Color(0.9f, 1, 0) },
        { JsonObjects.Stats.Strength, new Color(1, 0, 0.8f) },
        { JsonObjects.Stats.Agility, new Color(0, 1, 0.3f) },
        { JsonObjects.Stats.Intelligence, new Color(0, 0.5f, 1) },
        { JsonObjects.Stats.Wisdom, new Color(0, 1, 1) }
    };

    private void InitStats(List<JsonObjects.Items.ItemStat> stats)
    {
        foreach (Transform child in Stats.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var stat in stats)
        {
            var statObj = Instantiate(StatUI, Stats.transform);
            var script = statObj.GetComponent<StatUI>();

            script.Label.text = string.Format(" * {0}{1}:", stat.Type, _statSpaces[stat.Type]);
            script.Label.color = _statColors[stat.Type];
            script.Value.text = string.Format("[+{0}]", stat.StatValue);
            script.Value.color = _statColors[stat.Type];
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
