using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureUI : MonoBehaviour
{
    public Text Day;

    public void SetDay(int day)
    {
        Day.text = $"{day}";
    }
}
