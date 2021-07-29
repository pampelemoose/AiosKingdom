using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public string MapIdentifier;
    public string MapName;

    public Vector2 SpawnPosition;

    private bool _mapLoaded = false;

    void Awake()
    {
        
    }

    void Update()
    {
        if (!_mapLoaded && AdventureUIManager.This != null)
        {
            AdventureUIManager.This.SetPlayerCurrentMap(MapName);

            _mapLoaded = true;
        }
    }
}