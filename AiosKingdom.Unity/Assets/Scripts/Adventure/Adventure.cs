using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Adventure : MonoBehaviour, ICallbackHooker
{
    [Header("General")]
    public Text Name;
    public Text Room;

    public GameObject Menu;
    public GameObject List;
    public Text EnemyTimer;
    public GameObject StatsPanel;

    [Header("Actions")]
    public Button CommandsButton;
    public Button LogsButton;
    public Button StatsButton;
    public Button AttackButton;
    public Button ConsumableButton;
    public Button WaitButton;
    public Button NextRoomButton;
    public Button FinishButton;
    public Button BackToMainButton;
    public Button Exit;

    [Header("Headers")]
    public GameObject EnemiesHeader;
    public GameObject LootsHeader;
    public GameObject ShopHeader;
    public GameObject SkillsHeader;
    public GameObject ConsumablesHeader;

    [Header("Items")]
    public ItemDetails ItemDetailPanel;

    [Header("Knowledges")]
    public KnowledgeDetails KnowledgeDetailPanel;

    [Header("Loots")]
    public GameObject LootItemSlot;

    [Header("Shop")]
    public GameObject ShopItemSlot;

    [Header("Combat")]
    public GameObject EnemyListItem;
    public GameObject KnowledgeListItem;
    public GameObject ConsumableListItem;

    [Header("Rest")]
    public GameObject RestInfos;

    [Header("Stats")]
    public Text Health;
    public Text Mana;
    public Text Stamina;
    public Text Energy;
    public Text Strength;
    public Text Agility;
    public Text Intelligence;
    public Text Wisdom;
    public Text Damages;

    [Header("Logs")]
    public LogBox LogBox;

    [Header("Results")]
    public GameObject ResultPrefab;
    public GameObject EndResultBox;
    public GameObject EndResultList;

    [Header("Pagination")]
    public GameObject PaginationPrefab;
    public GameObject PaginationBox;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private Dictionary<Guid, JsonObjects.AdventureState.EnemyState> _enemies;
    private Dictionary<Guid, JsonObjects.AdventureState.ShopState> _shopItems;
    private List<JsonObjects.Skills.BuiltSkill> _skills;
    private List<JsonObjects.AdventureState.BagItem> _bag;
    private List<JsonObjects.LootItem> _loots;

    private Guid _selectedEnemy;
    private GameObject _selectedEnemyObj;

    private bool _showActionPanel = true;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.Enter, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                InputController.This.SetId("Adventure");

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(true);
                });
            }
            else
            {
                Debug.Log("Dungeon Enter error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.EnterRoom, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.UpdateDungeonRoom();
            }
            else
            {
                Debug.Log("Dungeon Enter Room error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.Exit, (message) =>
        {

            if (message.Success)
            {
                InputController.This.SetId("Adventures");

                NetworkManager.This.AskCurrencies();
                NetworkManager.This.AskInventory();
                NetworkManager.This.AskSoulCurrentDatas();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowLoading();
                    gameObject.SetActive(false);
                });
            }
            else
            {
                Debug.Log("Dungeon Exit error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.UpdateRoom, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                var adventure = JsonConvert.DeserializeObject<JsonObjects.AdventureState>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updateCurrentState(adventure);
                });
            }
            else
            {
                Debug.Log("Dungeon Update Room error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.UseSkill, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.UpdateDungeonRoom();

                var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _logResults(arList);
                    _triggerEnemyTurn();
                });
            }
            else
            {
                Debug.Log("Dungeon Use Skill error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.UseConsumable, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.UpdateDungeonRoom();

                var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _logResults(arList);
                    _triggerEnemyTurn();
                });
            }
            else
            {
                Debug.Log("Dungeon Use Consumable error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.EnemyTurn, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.UpdateDungeonRoom();

                var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _logResults(arList);
                });
            }
            else
            {
                Debug.Log("Dungeon Enemy Turn error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.DoNothingTurn, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.UpdateDungeonRoom();

                var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _logResults(arList);
                    _triggerEnemyTurn();
                });
            }
            else
            {
                Debug.Log("Dungeon Do Nothing Turn error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.GetLoots, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                var loots = JsonConvert.DeserializeObject<List<JsonObjects.LootItem>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _showLoots(loots);
                });
            }
            else
            {
                Debug.Log("Dungeon Get Loots error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.LootItem, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.GetDungeonRoomLoots();
            }
            else
            {
                Debug.Log("Dungeon Loot Item error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.LeaveFinishedRoom, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                NetworkManager.This.AskKnowledges();

                var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _showEndResults(arList);
                });
            }
            else
            {
                Debug.Log("Dungeon Leave Finished Room error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.BuyShopItem, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.AskCurrencies();
                NetworkManager.This.UpdateDungeonRoom();
            }
            else
            {
                Debug.Log("Dungeon Buy Shop Item error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.PlayerDied, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _showEndResults(arList);
                });
            }
            else
            {
                Debug.Log("Dungeon Player Died error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Dungeon.PlayerRest, (message) =>
        {
            if (message.Success)
            {
                NetworkManager.This.OpenDungeonRoom();

                var arList = JsonConvert.DeserializeObject<List<JsonObjects.AdventureState.ActionResult>>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _logResults(arList);
                });
            }
            else
            {
                Debug.Log("Dungeon Player Rest error : " + message.Json);
            }
        });

        InputController.This.AddCallback("Adventure", (direction) =>
        {
            if (gameObject.activeSelf)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    if (direction == SwipeDirection.Up)
                        Menu.SetActive(true);
                    if (direction == SwipeDirection.Down)
                        Menu.SetActive(false);
                });
            }
        });
    }

    void Awake()
    {
        CommandsButton.onClick.RemoveAllListeners();
        CommandsButton.onClick.AddListener(() =>
        {
            Menu.SetActive(!Menu.activeSelf);
        });

        Exit.onClick.RemoveAllListeners();
        Exit.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowLoading();
            });

            Menu.SetActive(false);
            NetworkManager.This.ExitDungeon();
            gameObject.SetActive(false);
        });

        LogsButton.onClick.RemoveAllListeners();
        LogsButton.onClick.AddListener(() =>
        {
            Menu.SetActive(false);
            LogBox.ShowLogs();
        });

        StatsButton.onClick.RemoveAllListeners();
        StatsButton.onClick.AddListener(() =>
        {
            Menu.SetActive(false);
            StatsPanel.SetActive(true);
        });

        AttackButton.onClick.RemoveAllListeners();
        AttackButton.onClick.AddListener(() =>
        {
            Menu.SetActive(false);
            _setHeaders("skills");

            _skills = DatasManager.Instance.Adventure.State.Skills;

            _pagination.Setup(ItemPerPage, _skills.Count, _setSkills);

            _setSkills();
        });

        ConsumableButton.onClick.RemoveAllListeners();
        ConsumableButton.onClick.AddListener(() =>
        {
            Menu.SetActive(false);
            _setHeaders("consumables");

            _bag = DatasManager.Instance.Adventure.Bag;

            _pagination.Setup(ItemPerPage, _bag.Count, _setConsumables);

            _setConsumables();
        });

        WaitButton.onClick.RemoveAllListeners();
        WaitButton.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowLoading();
            });

            Menu.SetActive(false);
            _showActionPanel = false;

            _resetActions();

            NetworkManager.This.DoNothingTurn();
        });

        NextRoomButton.onClick.RemoveAllListeners();
        NextRoomButton.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowLoading();
            });

            Menu.SetActive(false);
            NetworkManager.This.OpenDungeonRoom();
        });

        FinishButton.onClick.RemoveAllListeners();
        FinishButton.onClick.AddListener(() =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.ShowLoading();
            });

            Menu.SetActive(false);
            _showActionPanel = true;
            NetworkManager.This.LeaveFinishedRoom();
        });

        BackToMainButton.onClick.RemoveAllListeners();
        BackToMainButton.onClick.AddListener(() =>
        {
            InputController.This.SetId("Adventures");
            Menu.SetActive(false);
            NetworkManager.This.DungeonLeft();
            gameObject.SetActive(false);
        });

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }

        NetworkManager.This.UpdateDungeonRoom();
        LogBox.ClearLogs();
    }

    private void _updateCurrentState(JsonObjects.AdventureState state)
    {
        var adventure = state;

        Name.text = adventure.Name;
        Room.text = string.Format("[{0}/{1}]", adventure.CurrentRoom, adventure.TotalRoomCount);

        _resetActions();
        RestInfos.SetActive(false);
        EndResultBox.SetActive(false);
        if (adventure.IsFightArea)
        {
            _setHeaders("enemies");

            if (adventure.Enemies.Count == 0 && !adventure.IsExit)
            {
                NetworkManager.This.GetDungeonRoomLoots();
            }

            _enemies = adventure.Enemies;
            _pagination.Setup(ItemPerPage, _enemies.Count, _setCombat);

            if (_showActionPanel) _showCombatActions();
            else _showEnemyTurnActions();

            _setCombat();
        }
        else if (adventure.IsShopArea)
        {
            _setHeaders("shop");

            _shopItems = adventure.Shops;
            _pagination.Setup(ItemPerPage, _shopItems.Count, _setShop);

            _showShoppingActions();
            _setShop();
        }
        else if (adventure.IsRestingArea)
        {
            _setHeaders("none");

            foreach (Transform child in List.transform)
            {
                Destroy(child.gameObject);
            }

            RestInfos.SetActive(true);

            NetworkManager.This.PlayerRest();
        }
        else if (adventure.IsExit)
        {
            _setHeaders("none");

            foreach (Transform child in List.transform)
            {
                Destroy(child.gameObject);
            }

            FinishButton.gameObject.SetActive(true);
        }

        Health.text = string.Format("[{0}/{1}]", adventure.State.CurrentHealth, adventure.State.MaxHealth);
        Mana.text = string.Format("[{0}/{1}]", adventure.State.CurrentMana, adventure.State.MaxMana);

        Stamina.text = string.Format("[{0}]", adventure.State.Stamina);
        Energy.text = string.Format("[{0}]", adventure.State.Energy);
        Strength.text = string.Format("[{0}]", adventure.State.Strength);
        Agility.text = string.Format("[{0}]", adventure.State.Agility);
        Intelligence.text = string.Format("[{0}]", adventure.State.Intelligence);
        Wisdom.text = string.Format("[{0}]", adventure.State.Wisdom);

        Damages.text = string.Format("[{0}/{1}]", adventure.State.MinDamages, adventure.State.MaxDamages);
    }

    private void _setHeaders(string toShow)
    {
        EnemiesHeader.SetActive(false);
        LootsHeader.SetActive(false);
        ShopHeader.SetActive(false);
        SkillsHeader.SetActive(false);
        ConsumablesHeader.SetActive(false);

        switch (toShow)
        {
            case "enemies":
                EnemiesHeader.SetActive(true);
                break;
            case "shop":
                ShopHeader.SetActive(true);
                break;
            case "loots":
                LootsHeader.SetActive(true);
                break;
            case "skills":
                SkillsHeader.SetActive(true);
                break;
            case "consumables":
                ConsumablesHeader.SetActive(true);
                break;
        }
    }

    private void _resetActions()
    {
        AttackButton.gameObject.SetActive(false);
        ConsumableButton.gameObject.SetActive(false);
        WaitButton.gameObject.SetActive(false);
        EnemyTimer.gameObject.SetActive(false);
        NextRoomButton.gameObject.SetActive(false);
        FinishButton.gameObject.SetActive(false);
        BackToMainButton.gameObject.SetActive(false);
    }

    private void _showCombatActions()
    {
        AttackButton.gameObject.SetActive(true);

        if (DatasManager.Instance.Adventure.Bag.Count > 0)
            ConsumableButton.gameObject.SetActive(true);

        WaitButton.gameObject.SetActive(true);
    }

    private void _showShoppingActions()
    {
        NextRoomButton.gameObject.SetActive(true);
    }

    private void _showEnemyTurnActions()
    {
        EnemyTimer.gameObject.SetActive(true);
    }

    private void _showLootingActions()
    {
        NextRoomButton.gameObject.SetActive(true);
    }

    private void _logResults(List<JsonObjects.AdventureState.ActionResult> actionResults)
    {
        foreach (var result in actionResults)
        {
            LogBox.AddLogs(result);
        }
    }

    private void _showLoots(List<JsonObjects.LootItem> loots)
    {
        _setHeaders("loots");

        _resetActions();

        _showLootingActions();

        _loots = loots;

        _pagination.Setup(ItemPerPage, _loots.Count, _setLoots);

        _setLoots();
    }

    private void _setLoots()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var loots = _loots.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var loot in loots)
        {
            var newObj = Instantiate(LootItemSlot, List.transform);
            var script = newObj.GetComponent<LootListItem>();
            var item = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(loot.ItemId));
            script.Initialize(item, loot);
            script.Loot.onClick.AddListener(() =>
            {
                NetworkManager.This.LootDungeonItem(loot.LootId);
            });
            script.Action.onClick.AddListener(() =>
            {
                ItemDetailPanel.ShowDetails(item);
            });
        }

        _pagination.SetIndicator((_loots.Count / ItemPerPage) + (_loots.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void _triggerEnemyTurn()
    {
        var adventure = DatasManager.Instance.Adventure;

        if (adventure.Enemies.Count > 0)
            StartCoroutine(_executeEnemyTurn());
    }

    private IEnumerator _executeEnemyTurn()
    {
        for (int i = 0; i < 5; ++i)
        {
            EnemyTimer.text = string.Format("+[ Enemy turn in {0} seconds ]+", 5 - i);
            yield return new WaitForSeconds(1.0f);
        }

        NetworkManager.This.EnemyTurn();
        _showActionPanel = true;
    }

    private void _showEndResults(List<JsonObjects.AdventureState.ActionResult> actionResults)
    {
        _resetActions();
        BackToMainButton.gameObject.SetActive(true);
        EndResultBox.SetActive(true);

        foreach (Transform child in EndResultList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var result in actionResults)
        {
            var newObj = Instantiate(ResultPrefab, EndResultList.transform);
            var script = newObj.GetComponent<ResultListItem>();
            script.SetDatas(result);
        }
    }

    private void _setCombat()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var enemies = _enemies.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        if (!enemies.Any(e => e.Key.Equals(_selectedEnemy)))
        {
            _selectedEnemy = Guid.Empty;
        }

        foreach (var enemy in enemies)
        {
            var ennemyObj = Instantiate(EnemyListItem, List.transform);
            var script = ennemyObj.GetComponent<EnemyListItem>();
            script.Initialize(enemy.Value);
            script.Action.onClick.AddListener(() =>
            {
                if (!_selectedEnemy.Equals(Guid.Empty) && !_selectedEnemy.Equals(enemy.Key))
                {
                    script.Select();
                    _selectedEnemyObj.GetComponent<EnemyListItem>().Unselect();
                    _selectedEnemy = enemy.Key;
                    _selectedEnemyObj = ennemyObj;
                }
            });

            if (_selectedEnemy.Equals(Guid.Empty) || _selectedEnemy.Equals(enemy.Key))
            {
                script.Select();
                _selectedEnemy = enemy.Key;
                _selectedEnemyObj = ennemyObj;
            }
            else
            {
                script.Unselect();
            }
        }

        _pagination.SetIndicator((_enemies.Count / ItemPerPage) + (_enemies.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void _setShop()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var shopItems = _shopItems.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var shopItem in shopItems)
        {
            var newObj = Instantiate(ShopItemSlot, List.transform);
            var script = newObj.GetComponent<ShopListItem>();
            var item = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(shopItem.Value.ItemId));
            script.Initialize(item, shopItem.Value);
            script.Buy.onClick.AddListener(() =>
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowLoading();
                });

                NetworkManager.This.BuyShopItem(shopItem.Key, 1);
            });
            script.Action.onClick.AddListener(() =>
            {
                ItemDetailPanel.ShowDetails(item);
            });
        }

        _pagination.SetIndicator((_shopItems.Count / ItemPerPage) + (_shopItems.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void _setSkills()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var skills = _skills.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var skill in skills)
        {
            var knowledgeObj = Instantiate(KnowledgeListItem, List.transform);
            var script = knowledgeObj.GetComponent<SkillSelectionListItem>();
            script.SetDatas(skill);
            script.Use.onClick.AddListener(() =>
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowLoading();
                });

                _showActionPanel = false;

                _resetActions();

                NetworkManager.This.AdventureUseSkill(skill.Id, _selectedEnemy);
            });
            script.Action.onClick.AddListener(() =>
            {
                KnowledgeDetailPanel.ShowBuiltDetails(skill);
            });
        }

        _pagination.SetIndicator((_skills.Count / ItemPerPage) + (_skills.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void _setConsumables()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var bagItems = _bag.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var bagItem in bagItems)
        {
            var item = DatasManager.Instance.Items.FirstOrDefault(b => b.Id.Equals(bagItem.ItemId));
            var consumableItemObj = Instantiate(ConsumableListItem, List.transform);
            var script = consumableItemObj.GetComponent<ConsumableListItem>();
            script.SetDatas(bagItem);
            script.Use.onClick.AddListener(() =>
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowLoading();
                });

                _showActionPanel = false;

                _resetActions();

                NetworkManager.This.AdventureUseConsumable(bagItem.InventoryId, _selectedEnemy);
            });
            script.Action.onClick.AddListener(() =>
            {
                ItemDetailPanel.ShowDetails(item);
            });
        }

        _pagination.SetIndicator((_bag.Count / ItemPerPage) + (_bag.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
