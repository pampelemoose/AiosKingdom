using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class UIHelper
{
    public static IEnumerator SetScrollviewVerticalSize(GameObject obj)
    {
        yield return new WaitForEndOfFrame();

        var layout = obj.GetComponent<VerticalLayoutGroup>();
        var rectTransform = obj.GetComponent<RectTransform>();
        float size = 0.0f;

        foreach (Transform child in obj.transform)
        {
            var childRectTransform = child.GetComponent<RectTransform>();
            size += childRectTransform.sizeDelta.y + (layout != null ? layout.spacing : 0.0f);
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, size);
    }

    public static IEnumerator SetScrollviewHorizontalSize(GameObject obj)
    {
        yield return new WaitForEndOfFrame();

        var layout = obj.GetComponent<HorizontalLayoutGroup>();
        var rectTransform = obj.GetComponent<RectTransform>();
        float size = 0.0f;

        foreach (Transform child in obj.transform)
        {
            var childRectTransform = child.GetComponent<RectTransform>();
            size += childRectTransform.sizeDelta.x + (layout != null ? layout.spacing : 0.0f);
        }

        rectTransform.sizeDelta = new Vector2(size, rectTransform.sizeDelta.y);
    }
}
