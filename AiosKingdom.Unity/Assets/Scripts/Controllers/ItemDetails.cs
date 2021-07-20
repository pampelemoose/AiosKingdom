using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetails : PaginationBox
{
    [Header("Item Details")]
    public Image BorderImage;
    public Text Name;
    public Text Type;
    public Text Quality;
    public Text ItemLevel;
    public Text RequiredLevel;
    public GameObject Special;
    public Text SpecialLabel;
    public Text SpecialValue;
    public GameObject SpecialTwo;
    public Text SpecialTwoLabel;
    public Text SpecialTwoValue;
    public GameObject Stats;
    public GameObject StatUI;
    public GameObject EffectUI;
    public EffectDetails EffectDetailPanel;

    public Button Close;

    private List<JsonObjects.Items.ItemStat> _stats;
    private List<JsonObjects.Items.ItemEffect> _effects;

    private void Awake()
    {
        Close.onClick.RemoveAllListeners();
        Close.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void ShowDetails(JsonObjects.Items.Item item)
    {
        InitAitem(item);

        switch (item.Type)
        {
            case JsonObjects.Items.ItemType.Armor:
                {
                    SpecialLabel.text = "AR:";
                    SpecialValue.text = $"{item.ArmorValue}";
                    SpecialTwoLabel.text = "MR:";
                    SpecialTwoValue.text = $"{item.MagicArmorValue}";

                    Type.text = item.Slot.ToString();

                    InitStats(item.Stats);
                }
                break;
            case JsonObjects.Items.ItemType.Bag:
                {
                    SpecialLabel.text = "SLT:";
                    SpecialValue.text = $"{item.SlotCount}";
                    SpecialTwo.SetActive(false);

                    Type.text = "Bag";

                    InitStats(item.Stats);
                }
                break;
            case JsonObjects.Items.ItemType.Consumable:
                {
                    Special.SetActive(false);
                    SpecialTwo.SetActive(false);

                    Type.text = "Consumable";

                    InitEffects(item.Effects);
                }
                break;
            default:
                {
                    SpecialLabel.text = "Min.Dmg:";
                    SpecialValue.text = $"{item.MinDamages}";
                    SpecialTwoLabel.text = "Max.Dmg:";
                    SpecialTwoValue.text = $"{item.MaxDamages}";

                    Type.text = item.Type.ToString();

                    InitStats(item.Stats);
                }
                break;
        }
    }

    private void InitAitem(JsonObjects.Items.Item item)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = $"{item.Name}";
        Quality.text = item.Quality.ToString();
        Quality.color = UIManager.QualityColor[item.Quality];
        BorderImage.color = UIManager.QualityColor[item.Quality];

        ItemLevel.text = $"{item.ItemLevel}";
        RequiredLevel.text = $"{item.UseLevelRequired}";

        Special.SetActive(true);
        SpecialTwo.SetActive(true);
    }

    private void InitStats(List<JsonObjects.Items.ItemStat> stats)
    {
        _stats = stats;
        _effects = null;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, (_stats != null ? _stats.Count : 0), SetStats);

        SetStats();
    }

    private void SetStats()
    {
        foreach (Transform child in Stats.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedStats = _stats.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var stat in paginatedStats)
        {
            var statObj = Instantiate(StatUI, Stats.transform);
            var script = statObj.GetComponent<StatUI>();

            script.Label.text = $"{stat.Type}:";
            script.Label.color = UIManager.StatColors[stat.Type];
            script.Value.text = $"+{stat.StatValue}";
            script.Value.color = UIManager.StatColors[stat.Type];
        }

        _pagination.SetIndicator((_stats.Count / ItemPerPage) + (_stats.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void InitEffects(List<JsonObjects.Items.ItemEffect> effects)
    {
        _stats = null;
        _effects = effects;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, Box.transform);
            _pagination = pagination.GetComponent<Pagination>();
        }
        _pagination.Setup(ItemPerPage, (_effects != null ? _effects.Count : 0), SetEffects);

        SetEffects();
    }

    private void SetEffects()
    {
        foreach (Transform child in Stats.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedEffects = _effects.Skip((_pagination.CurrentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var effect in paginatedEffects)
        {
            var statObj = Instantiate(EffectUI, Stats.transform);
            var script = statObj.GetComponent<EffectUI>();

            script.Label.text = $"{effect.Name}:";
            script.More.onClick.AddListener(() =>
            {
                EffectDetailPanel.SetDatas(effect);
            });
        }

        _pagination.SetIndicator((_effects.Count / ItemPerPage) + (_effects.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
