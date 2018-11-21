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
        if (!Guid.Empty.Equals(equipment.Head))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var head = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Head));
            script.InitializeAsArmor(head);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(head);
            });
        }
        if (!Guid.Empty.Equals(equipment.Shoulder))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var shoulder = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Shoulder));
            script.InitializeAsArmor(shoulder);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(shoulder);
            });
        }
        if (!Guid.Empty.Equals(equipment.Torso))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var torso = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Torso));
            script.InitializeAsArmor(torso);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(torso);
            });
        }
        if (!Guid.Empty.Equals(equipment.Belt))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var belt = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Belt));
            script.InitializeAsArmor(belt);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(belt);
            });
        }
        if (!Guid.Empty.Equals(equipment.Pants))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var pants = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Pants));
            script.InitializeAsArmor(pants);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(pants);
            });
        }
        if (!Guid.Empty.Equals(equipment.Leg))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var leg = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Leg));
            script.InitializeAsArmor(leg);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(leg);
            });
        }
        if (!Guid.Empty.Equals(equipment.Feet))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var feet = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Feet));
            script.InitializeAsArmor(feet);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(feet);
            });
        }
        if (!Guid.Empty.Equals(equipment.Hand))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var hand = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Hand));
            script.InitializeAsArmor(hand);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.GetComponent<ItemDetails>().ShowArmorDetails(hand);
            });
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
