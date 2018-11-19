using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pagination : MonoBehaviour
{
    public Text Indicator;
    public Button Prev;
    public Button Next;

    public void SetIndicator(int current, int max)
    {
        if (max == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        Indicator.text = string.Format("[{0} / {1}]", current, max);
    }
}
