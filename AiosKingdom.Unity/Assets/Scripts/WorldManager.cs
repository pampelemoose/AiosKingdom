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

    public int StaminaConsumption => _currentStaminaConsumption;
    private int _currentStaminaConsumption;

    void Awake()
    {
        if (!_created)
        {
            This = this;
            _created = true;

            _worldMapsPrefabs = new Dictionary<string, GameObject>();
            foreach (var mapEntry in WorldMapPrefabs)
            {
                _worldMapsPrefabs.Add(mapEntry.Identifier, mapEntry.MapPrefab);
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetZoneConsumption(int val)
    {
        _currentStaminaConsumption = val;
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
        var adventureMovingState = DatasManager.Instance.Adventure.MovingState;

        AdventureUIManager.This.UpdateCharacterStats(adventureMovingState.CurrentStamina);
    }

    public void Move(JsonObjects.MovingState state)
    {
        var characterInputScript = Character.GetComponent<CharacterInput>();
        characterInputScript.Move(state);

        AdventureUIManager.This.UpdateCharacterStats(state.CurrentStamina);
    }

    public void EnterTavern(Guid tavernId)
    {
        var tavern = DatasManager.Instance.Taverns.FirstOrDefault(t => t.Id == tavernId);

        if (tavern != null)
        {
            AdventureUIManager.This.EnterTavern(tavern);
        }
    }

    public void EnterBookstore(Guid bookstoreId)
    {
        var bookstore = DatasManager.Instance.Bookstores.FirstOrDefault(t => t.Id == bookstoreId);

        if (bookstore != null)
        {
            AdventureUIManager.This.EnterBookstore(bookstore);
        }
    }

    public void RestInTavern(JsonObjects.MovingState state)
    {
        AdventureUIManager.This.UpdateCharacterStats(state.CurrentStamina);
    }
}
