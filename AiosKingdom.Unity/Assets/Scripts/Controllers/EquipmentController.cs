using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentController : MonoBehaviour
{
    public GameObject ListBox;
    public GameObject EquipmentListItemPrefab;

    public Button Close;

    public ItemDetails ItemDetails;

    void Awake()
    {
        UpdateEquipment();

        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            UIManager.This.ShowMain();
        });
    }

    public void UpdateEquipment()
    {
        foreach (Transform child in ListBox.transform)
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
                var newObj = Instantiate(EquipmentListItemPrefab, ListBox.transform);
                var script = newObj.GetComponent<EquipmentListItem>();
                var head = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(armorId));
                script.SetData(head);
                script.ShowDetailsButton.onClick.AddListener(() =>
                {
                    ItemDetails.ShowDetails(head);
                });
            }
        }
        
        if (!Guid.Empty.Equals(equipment.WeaponLeft))
        {
            var newObj = Instantiate(EquipmentListItemPrefab, ListBox.transform);
            var script = newObj.GetComponent<EquipmentListItem>();
            var leftHand = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.WeaponLeft));
            script.SetData(leftHand);
            script.ShowDetailsButton.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(leftHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.WeaponRight))
        {
            var newObj = Instantiate(EquipmentListItemPrefab, ListBox.transform);
            var script = newObj.GetComponent<EquipmentListItem>();
            var rightHand = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.WeaponRight));
            script.SetData(rightHand);
            script.ShowDetailsButton.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(rightHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.Bag))
        {
            var bagObj = Instantiate(EquipmentListItemPrefab, ListBox.transform);
            var script = bagObj.GetComponent<EquipmentListItem>();
            var bag = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Bag));
            script.SetData(bag);
            script.ShowDetailsButton.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(bag);
            });
        }
    }
}
