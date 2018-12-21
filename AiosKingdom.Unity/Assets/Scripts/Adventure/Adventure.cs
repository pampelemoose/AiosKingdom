using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Adventure : MonoBehaviour
{
    [Header("General")]
    public Text Name;
    public Text Room;
    public Button Exit;
    public GameObject List;
    public Button LogsButton;
    public Button StatsButton;
    public GameObject StatsPanel;

    [Header("Actions")]
    public Button AttackButton;
    public Button ConsumableButton;
    public Button WaitButton;
    public Button NextRoomButton;
    public Button FinishButton;
    public Button BackToMainButton;
    public Text EnemyTimer;

    [Header("Headers")]
    public GameObject EnemiesHeader;
    public GameObject LootsHeader;
    public GameObject ShopHeader;
    public GameObject SkillsHeader;
    public GameObject ConsumablesHeader;

    [Header("Items")]
    public GameObject ItemDetailPanel;

    [Header("Knowledges")]
    public GameObject KnowledgeDetailPanel;

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

    [Header("Enemy Stats")]
    public GameObject EnemyBox;
    public Text EnemyHealth;
    public Text EnemyDamages;
    public Text EnemyPhase;

    [Header("Logs")]
    public GameObject LogBox;

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
    private List<JsonObjects.Knowledge> _knowledges;
    private List<JsonObjects.AdventureState.BagItem> _bag;
    private List<JsonObjects.LootItem> _loots;

    private Guid _selectedEnemy;
    private GameObject _selectedEnemyObj;

    private bool _showActionPanel = true;

    void Awake()
    {
        Exit.onClick.RemoveAllListeners();
        Exit.onClick.AddListener(() =>
        {
            NetworkManager.This.ExitDungeon();
            gameObject.SetActive(false);
        });

        LogsButton.onClick.RemoveAllListeners();
        LogsButton.onClick.AddListener(() =>
        {
            LogBox.GetComponent<LogBox>().ShowLogs();
        });

        StatsButton.onClick.RemoveAllListeners();
        StatsButton.onClick.AddListener(() =>
        {
            StatsPanel.SetActive(true);
        });

        AttackButton.onClick.RemoveAllListeners();
        AttackButton.onClick.AddListener(() =>
        {
            SetHeaders("skills");

            _knowledges = DatasManager.Instance.Knowledges;

            _pagination.Setup(ItemPerPage, _knowledges.Count, SetSkills);

            SetSkills();
        });

        ConsumableButton.onClick.RemoveAllListeners();
        ConsumableButton.onClick.AddListener(() =>
        {
            SetHeaders("consumables");

            _bag = DatasManager.Instance.Adventure.Bag;

            _pagination.Setup(ItemPerPage, _bag.Count, SetConsumables);

            SetConsumables();
        });

        WaitButton.onClick.RemoveAllListeners();
        WaitButton.onClick.AddListener(() =>
        {
            _showActionPanel = false;

            ResetActions();

            NetworkManager.This.DoNothingTurn();
        });

        NextRoomButton.onClick.RemoveAllListeners();
        NextRoomButton.onClick.AddListener(() =>
        {
            NetworkManager.This.OpenDungeonRoom();
        });

        FinishButton.onClick.RemoveAllListeners();
        FinishButton.onClick.AddListener(() =>
        {
            _showActionPanel = true;
            NetworkManager.This.LeaveFinishedRoom();
        });

        BackToMainButton.onClick.RemoveAllListeners();
        BackToMainButton.onClick.AddListener(() =>
        {
            NetworkManager.This.DungeonLeft();
            gameObject.SetActive(false);
            UIManager.This.ShowHome();
        });

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
    }

    public void Initialize()
    {
        NetworkManager.This.UpdateDungeonRoom();
        LogBox.GetComponent<LogBox>().ClearLogs();
    }

    public void UpdateCurrentState()
    {
        var adventure = DatasManager.Instance.Adventure;

        Name.text = adventure.Name;
        Room.text = string.Format("[{0}/{1}]", adventure.CurrentRoom, adventure.TotalRoomCount);

        ResetActions();
        RestInfos.SetActive(false);
        EnemyBox.SetActive(false);
        EndResultBox.SetActive(false);
        if (adventure.IsFightArea)
        {
            SetHeaders("enemies");

            if (adventure.Enemies.Count == 0 && !adventure.IsExit)
            {
                NetworkManager.This.GetDungeonRoomLoots();
            }

            _enemies = adventure.Enemies;
            _pagination.Setup(ItemPerPage, _enemies.Count, SetCombat);

            if (_showActionPanel) ShowCombatActions();
            else ShowEnemyTurnActions();

            SetCombat();
        }
        else if (adventure.IsShopArea)
        {
            SetHeaders("shop");

            _shopItems = adventure.Shops;
            _pagination.Setup(ItemPerPage, _shopItems.Count, SetShop);

            ShowShoppingActions();
            SetShop();
        }
        else if (adventure.IsRestingArea)
        {
            SetHeaders("none");

            foreach (Transform child in List.transform)
            {
                Destroy(child.gameObject);
            }

            RestInfos.SetActive(true);

            NetworkManager.This.PlayerRest();
        }
        else if (adventure.IsExit)
        {
            SetHeaders("none");

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

    private void SetHeaders(string toShow)
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

    private void ResetActions()
    {
        AttackButton.gameObject.SetActive(false);
        ConsumableButton.gameObject.SetActive(false);
        WaitButton.gameObject.SetActive(false);
        EnemyTimer.gameObject.SetActive(false);
        NextRoomButton.gameObject.SetActive(false);
        FinishButton.gameObject.SetActive(false);
        BackToMainButton.gameObject.SetActive(false);
    }

    private void ShowCombatActions()
    {
        AttackButton.gameObject.SetActive(true);

        if (DatasManager.Instance.Adventure.Bag.Count > 0)
            ConsumableButton.gameObject.SetActive(true);

        WaitButton.gameObject.SetActive(true);
    }

    private void ShowShoppingActions()
    {
        NextRoomButton.gameObject.SetActive(true);
    }

    private void ShowEnemyTurnActions()
    {
        EnemyTimer.gameObject.SetActive(true);
    }

    private void ShowLootingActions()
    {
        NextRoomButton.gameObject.SetActive(true);
    }

    public void LogResults(List<JsonObjects.AdventureState.ActionResult> actionResults)
    {
        var log = LogBox.GetComponent<LogBox>();

        foreach (var result in actionResults)
        {
            log.AddLogs(result);
        }
    }

    public void ShowLoots(List<JsonObjects.LootItem> loots)
    {
        SetHeaders("loots");

        ResetActions();

        ShowLootingActions();

        _loots = loots;

        _pagination.Setup(ItemPerPage, _loots.Count, SetLoots);

        SetLoots();
    }

    private void SetLoots()
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
                ItemDetailPanel.GetComponent<ItemDetails>().ShowDetails(item);
            });
        }

        _pagination.SetIndicator((_loots.Count / ItemPerPage) + (_loots.Count % ItemPerPage > 0 ? 1 : 0));
    }

    public void TriggerEnemyTurn()
    {
        var adventure = DatasManager.Instance.Adventure;

        if (adventure.Enemies.Count > 0)
            StartCoroutine(ExecuteEnemyTurn());
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        for (int i = 0; i < 5; ++i)
        {
            EnemyTimer.text = string.Format("+[ Enemy turn in {0} seconds ]+", 5 - i);
            yield return new WaitForSeconds(1.0f);
        }

        NetworkManager.This.EnemyTurn();
        _showActionPanel = true;
    }

    public void ShowEndResults(List<JsonObjects.AdventureState.ActionResult> actionResults)
    {
        ResetActions();
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

    private void SetCombat()
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

    private void SetShop()
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
                NetworkManager.This.BuyShopItem(shopItem.Key, 1);
            });
            script.Action.onClick.AddListener(() =>
            {
                ItemDetailPanel.GetComponent<ItemDetails>().ShowDetails(item);
            });
        }

        _pagination.SetIndicator((_shopItems.Count / ItemPerPage) + (_shopItems.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void SetSkills()
    {
        foreach (Transform child in List.transform)
        {
            Destroy(child.gameObject);
        }

        var knowledges = _knowledges.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var knowledge in knowledges)
        {
            var skill = DatasManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(knowledge.BookId));

            var knowledgeObj = Instantiate(KnowledgeListItem, List.transform);
            var script = knowledgeObj.GetComponent<SkillSelectionListItem>();
            script.SetDatas(knowledge);
            script.Use.onClick.AddListener(() =>
            {
                _showActionPanel = false;

                ResetActions();

                NetworkManager.This.AdventureUseSkill(knowledge.Id, _selectedEnemy);
            });
            script.Action.onClick.AddListener(() =>
            {
                KnowledgeDetailPanel.GetComponent<KnowledgeDetails>().ShowDetails(skill, knowledge.Rank);
            });
        }

        _pagination.SetIndicator((_knowledges.Count / ItemPerPage) + (_knowledges.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void SetConsumables()
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
                _showActionPanel = false;

                ResetActions();

                NetworkManager.This.AdventureUseConsumable(bagItem.InventoryId, _selectedEnemy);
            });
            script.Action.onClick.AddListener(() =>
            {
                ItemDetailPanel.GetComponent<ItemDetails>().ShowDetails(item);
            });
        }

        _pagination.SetIndicator((_bag.Count / ItemPerPage) + (_bag.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
