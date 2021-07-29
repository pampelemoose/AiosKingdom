using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDoor : MonoBehaviour
{
    public string GoToMapIdentifier;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"Colliding");

        if (collider.gameObject.tag == "MyPlayer")
        {
            Debug.Log($"Let's go to {GoToMapIdentifier}");
            //WorldManager.This.LoadMap(GoToMapIdentifier);
        }
    }
}
