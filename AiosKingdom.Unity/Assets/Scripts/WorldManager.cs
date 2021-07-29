using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject WorldParent;

    private Dictionary<string, GameObject> _worldMapsPrefabs;
    private string _currentMapIdentifier = null;
    private GameObject _currentMap;

    // IN TAVERN
    private JsonObjects.Adventures.Tavern _tavern;

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
            //if (_worldMapsPrefabs.ContainsKey(DefaultMapIdentifier))
            //{
            //    Character.SetActive(true);
            //    _currentMap = Instantiate(_worldMapsPrefabs[DefaultMapIdentifier], WorldParent.transform);
            //    var mapLoader = _currentMap.GetComponent<MapLoader>();
            //    var spawnAt = mapLoader.SpawnPosition;
            //    var character = Character.GetComponent<CharacterInput>();
            //    character.Spawn(spawnAt);

            //    _currentMapIdentifier = DefaultMapIdentifier;
            //}
        }
    }

    public void LoadMap(JsonObjects.Adventures.Adventure adventure)
    {
        var mapIdentifier = adventure.MapIdentifier.ToString().ToUpper();

        if (_worldMapsPrefabs.ContainsKey(mapIdentifier))
        {
            if (_currentMap != null)
            {
                Destroy(_currentMap);
            }

            _currentMap = Instantiate(_worldMapsPrefabs[mapIdentifier], WorldParent.transform);

            Character.transform.position = new Vector3(adventure.SpawnCoordinateX, adventure.SpawnCoordinateY, -1f);

            _currentMapIdentifier = mapIdentifier;
        }
    }

    public void LoadCharacter()
    {
        var soulData = DatasManager.Instance.Datas;

        var characterScript = Character.GetComponent<Character>();
        characterScript.Initialize(soulData);
    }

    public void Move(JsonObjects.Adventures.Movement move)
    {
        var characterInputScript = Character.GetComponent<CharacterInput>();
        characterInputScript.Move(move);
    }

    public void EnterTavern(Guid tavernId)
    {
        var tavern = DatasManager.Instance.Taverns.FirstOrDefault(t => t.Id == tavernId);

        if (tavern != null)
        {
            _tavern = tavern;
        }
    }

    public void ExitTavern()
    {
        _tavern = null;
    }

    public void RestInTavern()
    {
        if (_tavern != null)
        {
            var characterScript = Character.GetComponent<Character>();
            characterScript.RestoreStamina(_tavern.RestStamina);
        }
    }
}
