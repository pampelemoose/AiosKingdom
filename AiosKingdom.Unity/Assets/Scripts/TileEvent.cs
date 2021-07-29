using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEvent : MonoBehaviour
{
    public enum EventType
    {
        ZoneConsuption,
        EnterTavern,

        EnterCombat
    }

    public EventType Type;
    public int EventIntValue;
    public string EventStringValue;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "MyPlayer")
        {
            var characterScript = collider.GetComponent<Character>();
            if (!characterScript)
            {
                return;
            }

            switch (Type)
            {
                case EventType.ZoneConsuption:
                    characterScript.SetZoneConsumption(EventIntValue);
                    break;
                case EventType.EnterTavern:
                    WorldManager.This.EnterTavern(Guid.Parse(EventStringValue));
                    NetworkManager.This.RestInTavern(Guid.Parse(EventStringValue)); // TODO : TMP, NEED POPUP
                    break;
                case EventType.EnterCombat:
                    Debug.Log($"Enter combat with {EventStringValue}");
                    AdventureUIManager.This.StartCombat(characterScript.Health, characterScript.Health, characterScript.Mana, characterScript.Mana, null);
                    Destroy(gameObject);
                    break;
            }
        }
    }
}
