using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentListItem : MonoBehaviour
{
    public Image BorderImage;
    public Text NameText;
    public Text TypeText;
    public Text QualityText;
    public Text ItemLevelText;
    public Button ShowDetailsButton;

    public void SetData(JsonObjects.Items.Item item)
    {
        BorderImage.color = UIManager.ItemQualityColor[item.Quality];

        NameText.text = item.Name;

        TypeText.text = item.Type.ToString();
        if (item.Type == JsonObjects.Items.ItemType.Armor)
        {
            TypeText.text = item.Slot.ToString();
        }

        QualityText.text = item.Quality.ToString();
        QualityText.color = UIManager.ItemQualityColor[item.Quality];
        ItemLevelText.text = $"{item.ItemLevel}";

        ShowDetailsButton.onClick.RemoveAllListeners();
    }
}
