using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Serializable]
    public struct MapEntry
    {
        public string Identifier;
        public GameObject MapPrefab;
    }

    public static WorldManager This { get; set; }
    private static bool _created = false;

    public GameObject Character;
    public MapEntry[] WorldMapPrefabs;
    public string DefaultMapIdentifier;
    public GameObject WorldParent;

    private Dictionary<string, GameObject> _worldMapsPrefabs;
    private string _currentMapIdentifier = null;
    private GameObject _currentMap;

    void Awake()
    {
        if (!_created)
        {
            This = this;
            _created = true;
            _loadManager();
            
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void _loadManager()
    {
        _worldMapsPrefabs = new Dictionary<string, GameObject>();
        foreach (var mapEntry in WorldMapPrefabs)
        {
            _worldMapsPrefabs.Add(mapEntry.Identifier, mapEntry.MapPrefab);
        }

        if (_currentMapIdentifier == null)
        {
            if (_worldMapsPrefabs.ContainsKey(DefaultMapIdentifier))
            {
                Character.SetActive(true);
                _currentMap = Instantiate(_worldMapsPrefabs[DefaultMapIdentifier], WorldParent.transform);
                var mapLoader = _currentMap.GetComponent<MapLoader>();
                var spawnAt = mapLoader.SpawnPosition;
                var character = Character.GetComponent<CharacterInput>();
                character.Spawn(spawnAt);

                _currentMapIdentifier = DefaultMapIdentifier;
            }
        }
    }

    public void LoadMap(string identifier)
    {
        if (_worldMapsPrefabs.ContainsKey(identifier))
        {
            Destroy(_currentMap);

            _currentMap = Instantiate(_worldMapsPrefabs[identifier], WorldParent.transform);
            var mapLoader = _currentMap.GetComponent<MapLoader>();
            var spawnAt = mapLoader.SpawnPosition;
            Character.transform.position = new Vector3(spawnAt.x, spawnAt.y, -1f);

            _currentMapIdentifier = identifier;
        }
    }
}
