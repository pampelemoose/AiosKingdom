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
    public GameObject EffectDetailPanel;

    public GameObject PaginationBox;
    public GameObject PaginationPrefab;
    public int ItemPerPage = 5;

    private Pagination _pagination;
    private int _currentPage = 1;

    private List<JsonObjects.Items.ItemStat> _stats;
    private List<JsonObjects.Items.ItemEffect> _effects;

    public void ShowArmorDetails(JsonObjects.Items.Item armor)
    {
        InitAitem(armor);

        SpecialLabel.text = " * Armor        :";
        SpecialValue.text = string.Format("[{0}]", armor.ArmorValue);

        Type.text = string.Format("Armor - {0}", armor.Slot);

        InitStats(armor.Stats);
    }

    public void ShowWeaponsDetails(JsonObjects.Items.Item weapon)
    {
        InitAitem(weapon);

        SpecialLabel.text = " * Damages      :";
        SpecialValue.text = string.Format("[{0} - {1}]", weapon.MinDamages, weapon.MaxDamages);

        Type.text = string.Format("{0} - {1}", weapon.Slot, weapon.Type);

        InitStats(weapon.Stats);
    }

    public void ShowBagDetails(JsonObjects.Items.Item bag)
    {
        InitAitem(bag);

        SpecialLabel.text = " * Slots        :";
        SpecialValue.text = string.Format("[{0}]", bag.SlotCount);

        Type.text = "Bag";

        InitStats(bag.Stats);
    }

    public void ShowConsumableDetails(JsonObjects.Items.Item consumable)
    {
        InitAitem(consumable);

        SpecialLabel.gameObject.SetActive(false);
        SpecialValue.gameObject.SetActive(false);

        Type.text = "Cons.";

        InitEffects(consumable.Effects);
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

        SetupPagination();

        SetStats();
    }

    private void SetStats()
    {
        foreach (Transform child in Stats.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedStats = _stats.Skip((_currentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var stat in paginatedStats)
        {
            var statObj = Instantiate(StatUI, Stats.transform);
            var script = statObj.GetComponent<StatUI>();

            script.Label.text = string.Format(" * {0}{1}:", stat.Type, _statSpaces[stat.Type]);
            script.Label.color = _statColors[stat.Type];
            script.Value.text = string.Format("[+{0}]", stat.StatValue);
            script.Value.color = _statColors[stat.Type];
        }

        _pagination.SetIndicator(_currentPage, (_stats.Count / ItemPerPage) + (_stats.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void InitEffects(List<JsonObjects.Items.ItemEffect> effects)
    {
        _stats = null;
        _effects = effects;

        SetupPagination();

        SetEffects();
    }

    private void SetEffects()
    {
        foreach (Transform child in Stats.transform)
        {
            Destroy(child.gameObject);
        }

        var paginatedEffects = _effects.Skip((_currentPage - 1) * ItemPerPage).Take(ItemPerPage).ToList();

        foreach (var effect in paginatedEffects)
        {
            var statObj = Instantiate(EffectUI, Stats.transform);
            var script = statObj.GetComponent<EffectUI>();

            script.Label.text = string.Format(" * {0}", effect.Name);
            script.More.onClick.AddListener(() =>
            {
                EffectDetailPanel.GetComponent<EffectDetails>().SetDatas(effect);
            });
        }

        _pagination.SetIndicator(_currentPage, (_effects.Count / ItemPerPage) + (_effects.Count % ItemPerPage > 0 ? 1 : 0));
    }

    private void SetupPagination()
    {
        if (_pagination == null)
        {
            var pagination = Instantiate(PaginationPrefab, PaginationBox.transform);
            _pagination = pagination.GetComponent<Pagination>();

            _pagination.Prev.onClick.AddListener(() =>
            {
                if (_currentPage - 1 == 1)
                {
                    _pagination.Prev.gameObject.SetActive(false);
                }

                _pagination.Next.gameObject.SetActive(true);
                --_currentPage;

                if (_stats != null)
                {
                    SetStats();
                }
                else
                {
                    SetEffects();
                }
            });

            _pagination.Next.onClick.AddListener(() =>
            {
                if (_stats != null && (_stats.Count - ((_currentPage + 1) * ItemPerPage)) <= 0)
                {
                    _pagination.Next.gameObject.SetActive(false);
                }
                else if (_effects != null && (_effects.Count - ((_currentPage + 1) * ItemPerPage)) <= 0)
                {
                    _pagination.Next.gameObject.SetActive(false);
                }

                _pagination.Prev.gameObject.SetActive(true);
                ++_currentPage;

                if (_stats != null)
                {
                    SetStats();
                }
                else
                {
                    SetEffects();
                }
            });
        }

        _currentPage = 1;
        _pagination.Prev.gameObject.SetActive(false);
        if (_stats != null)
        {
            _pagination.Next.gameObject.SetActive(_stats.Count > ItemPerPage);
        }
        else
        {
            _pagination.Next.gameObject.SetActive(_effects.Count > ItemPerPage);
        }
    }
}
