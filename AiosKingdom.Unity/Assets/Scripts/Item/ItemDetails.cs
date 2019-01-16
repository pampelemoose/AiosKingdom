using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetails : MonoBehaviour
{
    public Text Name;
    public Text Type;
    public Text Quality;
    public Text ItemLevel;
    public Text RequiredLevel;
    public Text SpecialLabel;
    public Text SpecialValue;
    public GameObject Stats;
    public GameObject StatUI;
    public GameObject EffectUI;
    public EffectDetails EffectDetailPanel;

    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;

    private List<JsonObjects.Items.ItemStat> _stats;
    private List<JsonObjects.Items.ItemEffect> _effects;

    public void ShowDetails(JsonObjects.Items.Item item)
    {
        InitAitem(item);

        switch (item.Type)
        {
            case JsonObjects.Items.ItemType.Armor:
                {
                    SpecialLabel.text = " * Armor        :";
                    SpecialValue.text = string.Format("[{0}]", item.ArmorValue);

                    Type.text = string.Format("Armor - {0}", item.Slot);

                    InitStats(item.Stats);
                }
                break;
            case JsonObjects.Items.ItemType.Bag:
                {
                    SpecialLabel.text = " * Slots        :";
                    SpecialValue.text = string.Format("[{0}]", item.SlotCount);

                    Type.text = "Bag";

                    InitStats(item.Stats);
                }
                break;
            case JsonObjects.Items.ItemType.Consumable:
                {
                    SpecialLabel.gameObject.SetActive(false);
                    SpecialValue.gameObject.SetActive(false);

                    Type.text = "Cons.";

                    InitEffects(item.Effects);
                }
                break;
            default:
                {
                    SpecialLabel.text = " * Damages      :";
                    SpecialValue.text = string.Format("[{0} - {1}]", item.MinDamages, item.MaxDamages);

                    Type.text = string.Format("{0} - {1}", item.Slot, item.Type);

                    InitStats(item.Stats);
                }
                break;
        }
    }

    private void InitAitem(JsonObjects.Items.Item item)
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();

        Name.text = item.Name;
        Quality.text = item.Quality.ToString();

        ItemLevel.text = string.Format("[{0}]", item.ItemLevel);
        RequiredLevel.text = string.Format("[{0}]", item.UseLevelRequired);
    }

    private static Dictionary<JsonObjects.Stats, string> _statSpaces = new Dictionary<JsonObjects.Stats, string>
    {
        { JsonObjects.Stats.Stamina, "      " },
        { JsonObjects.Stats.Energy, "       " },
        { JsonObjects.Stats.Strength, "     " },
        { JsonObjects.Stats.Agility, "      " },
        { JsonObjects.Stats.Intelligence, " " },
        { JsonObjects.Stats.Wisdom, "       " }
    };
    private static Dictionary<JsonObjects.Stats, Color> _statColors = new Dictionary<JsonObjects.Stats, Color>
    {
        { JsonObjects.Stats.Stamina, new Color(1, 0, 0) },
        { JsonObjects.Stats.Energy, new Color(0.9f, 1, 0) },
        { JsonObjects.Stats.Strength, new Color(1, 0, 0.8f) },
        { JsonObjects.Stats.Agility, new Color(0, 1, 0.3f) },
        { JsonObjects.Stats.Intelligence, new Color(0, 0.5f, 1) },
        { JsonObjects.Stats.Wisdom, new Color(0, 1, 1) }
    };

    private void InitStats(List<JsonObjects.Items.ItemStat> stats)
    {
        _stats = stats;
        _effects = null;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
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

            script.Label.text = string.Format(" * {0}{1}:", stat.Type, _statSpaces[stat.Type]);
            script.Label.color = _statColors[stat.Type];
            script.Value.text = string.Format("[+{0}]", stat.StatValue);
            script.Value.color = _statColors[stat.Type];
        }

        _pagination.SetIndicator((_stats.Count / ItemPerPage) + (_stats.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void InitEffects(List<JsonObjects.Items.ItemEffect> effects)
    {
        _stats = null;
        _effects = effects;

        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
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

            script.Label.text = string.Format(" * {0}", effect.Name);
            script.More.onClick.AddListener(() =>
            {
                EffectDetailPanel.SetDatas(effect);
            });
        }

        _pagination.SetIndicator((_effects.Count / ItemPerPage) + (_effects.Count % ItemPerPage > 0 ? 1 : 0));
    }
}
