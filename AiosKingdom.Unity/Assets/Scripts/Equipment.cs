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
    public GameObject ItemDetails;

    public void UpdateEquipment()
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

        var equipment = DatasManager.Instance.Equipment;
        var equipmentSlots = new List<JsonObjects.Items.ItemSlot>
        {
            JsonObjects.Items.ItemSlot.Belt,
            JsonObjects.Items.ItemSlot.Feet,
            JsonObjects.Items.ItemSlot.Hand,
            JsonObjects.Items.ItemSlot.Head,
            JsonObjects.Items.ItemSlot.Leg,
            JsonObjects.Items.ItemSlot.Pants,
            JsonObjects.Items.ItemSlot.Torso,
        };

        foreach (var slot in equipmentSlots)
        {
            var armorId = equipment.GetArmorBySlot(slot);
            if (!Guid.Empty.Equals(armorId))
            {
                var newObj = Instantiate(ItemSlot, Content.transform);
                var script = newObj.GetComponent<ItemSlot>();
                var head = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(armorId));
                script.InitializeAsArmor(head);
                script.Action.onClick.AddListener(() =>
                {
                    ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(head);
                });
            }
        }
        
        if (!Guid.Empty.Equals(equipment.WeaponLeft))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var leftHand = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.WeaponLeft));
            script.InitializeAsWeapon(leftHand);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowWeaponsDetails(leftHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.WeaponRight))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var rightHand = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.WeaponRight));
            script.InitializeAsWeapon(rightHand);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowWeaponsDetails(rightHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.Bag))
        {
            var bagObj = Instantiate(ItemSlot, Content.transform);
            var script = bagObj.GetComponent<ItemSlot>();
            var bag = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Bag));
            script.InitializeAsBag(bag);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowBagDetails(bag);
            });
        }
    }
}
