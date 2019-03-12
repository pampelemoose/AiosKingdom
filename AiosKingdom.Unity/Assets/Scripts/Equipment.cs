using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour, ICallbackHooker
{
    public GameObject Content;
    public GameObject ItemSlot;

    public ItemDetails ItemDetails;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.Equipment, (message) =>
        {
            if (message.Success)
            {
                var equipment = JsonConvert.DeserializeObject<JsonObjects.Equipment>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updateEquipment(equipment);
                });
            }
            else
            {
                Debug.Log("Equipment error : " + message.Json);
            }
        });
    }

    public void _updateEquipment(JsonObjects.Equipment equipment)
    {
        foreach (Transform child in Content.transform)
        {
            Destroy(child.gameObject);
        }

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
                script.Initialize(head);
                script.Action.onClick.AddListener(() =>
                {
                    ItemDetails.ShowDetails(head);
                });
            }
        }
        
        if (!Guid.Empty.Equals(equipment.WeaponLeft))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var leftHand = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.WeaponLeft));
            script.Initialize(leftHand);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(leftHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.WeaponRight))
        {
            var newObj = Instantiate(ItemSlot, Content.transform);
            var script = newObj.GetComponent<ItemSlot>();
            var rightHand = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.WeaponRight));
            script.Initialize(rightHand);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(rightHand);
            });
        }
        if (!Guid.Empty.Equals(equipment.Bag))
        {
            var bagObj = Instantiate(ItemSlot, Content.transform);
            var script = bagObj.GetComponent<ItemSlot>();
            var bag = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(equipment.Bag));
            script.Initialize(bag);
            script.Action.onClick.AddListener(() =>
            {
                ItemDetails.ShowDetails(bag);
            });
        }
    }
}
