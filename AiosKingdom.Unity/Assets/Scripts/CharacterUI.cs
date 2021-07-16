using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public Text StaminaUI;

    public void SetStamina(int stamina)
    {
        StaminaUI.text = $"{stamina}";
    }    
}
