using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public Text StaminaUI;
    public Button OpenDetailsButton;

    public GameObject CharacterDetailsPopup;

    void Awake()
    {
        OpenDetailsButton.onClick.RemoveAllListeners();
        OpenDetailsButton.onClick.AddListener(() =>
        {
            var characterDetailScript = CharacterDetailsPopup.GetComponent<CharacterDetails>();
            characterDetailScript.Open();
        });
    }

    public void SetStamina(int stamina)
    {
        StaminaUI.text = $"{stamina}";
    }    
}
