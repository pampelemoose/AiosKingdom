using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBox : MonoBehaviour
{
    public GameObject MenuItemPrefab;

    public enum ActionType
    {
        Talk
    }

    private Dictionary<ActionType, GameObject> _instances;

    void Awake()
    {
        _instances = new Dictionary<ActionType, GameObject>();
    }

    public void AddAction(ActionType type, Action callback)
    {
        var menu = _getInstance(type);
        var menuScript = menu.GetComponent<MenuAction>();

        menuScript.Callback = callback;
        menuScript.SetText(Enum.GetName(typeof(ActionType), type));

        _formatMenu();
    }

    public void RemoveAction(ActionType type)
    {
        if (_instances.ContainsKey(type))
        {
            var menu = _instances[type];

            _instances.Remove(type);

            Destroy(menu);
        }

        _formatMenu();
    }

    private GameObject _getInstance(ActionType action)
    {
        if (!_instances.ContainsKey(action))
        {
            _instances.Add(action, Instantiate(MenuItemPrefab, gameObject.transform));
        }

        return _instances[action];
    }

    private void _formatMenu()
    {
        var rectTransform = GetComponent<RectTransform>();
        float size = 0.0f;

        foreach (Transform child in transform)
        {
            var childRectTransform = child.GetComponent<RectTransform>();
            size += childRectTransform.sizeDelta.y;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size);
    }
}