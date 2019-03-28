using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour, ICallbackHooker
{
    [Header("TopBar")]
    public Text Level;
    public Text Name;
    public Text Experience;

    [Space(2)]
    [Header("General")]
    public Text Health;
    public Text Mana;
    public Text Armor;
    public Text MagicArmor;
    public Text ItemLevel;

    [Space(2)]
    [Header("Offensive")]
    public Text MinDamages;
    public Text MaxDamages;

    [Space(2)]
    [Header("Attributes")]
    public Text Stamina;
    public Text Energy;
    public Text Strength;
    public Text Agility;
    public Text Intelligence;
    public Text Wisdom;
    public Button PointsAvailable;

    [Space(2)]
    [Header("Currencies")]
    public Text Spirits;
    public Text Embers;
    public Text Bits;
    public Text Shards;

    public void HookCallbacks()
    {
        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Listing.Recipes, (message) =>
        {
            if (message.Success)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    InputController.This.SetId("Home");
                    gameObject.SetActive(true);

                    NetworkManager.This.AskSoulCurrentDatas();
                    NetworkManager.This.AskCurrencies();
                    NetworkManager.This.AskEquipment();
                    NetworkManager.This.AskKnowledges();
                    NetworkManager.This.AskInventory();
                    NetworkManager.This.GetJob();
                });
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.CurrentSoulDatas, (message) =>
        {
            if (message.Success)
            {
                var soulDatas = JsonConvert.DeserializeObject<JsonObjects.SoulDatas>(message.Json);

                if (soulDatas.TotalStamina == 0 && soulDatas.TotalEnergy == 0
                    && soulDatas.TotalStrength == 0 && soulDatas.TotalAgility == 0
                    && soulDatas.TotalIntelligence == 0 && soulDatas.TotalWisdom == 0)
                {
                    // TODO : Tutorial
                    //Application.Current.Properties["AiosKingdom_TutorialStep"] = 3;
                    //Application.Current.SavePropertiesAsync();
                    //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
                }

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updatePlayerDatas(soulDatas);
                });
            }
            else
            {
                Debug.Log("Soul Datas error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.Market_OrderProcessed, (message) =>
        {
            if (message.Success)
            {
                var order = JsonConvert.DeserializeObject<JsonObjects.MarketOrderProcessed>(message.Json);
                var item = DatasManager.Instance.Items.FirstOrDefault(i => i.Id.Equals(order.ItemId));

                NetworkManager.This.AskInventory();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowAlert(string.Format("Your order has been processed. You received < {0} > * {1}.", item.Name, order.Quantity), "Market Order Processed");
                });
            }
            else
            {
                Debug.Log("Market Order Processed error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.EquipItem, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                NetworkManager.This.AskInventory();
                NetworkManager.This.AskEquipment();
                NetworkManager.This.AskSoulCurrentDatas();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowAlert(message.Json);
                });
            }
            else
            {
                Debug.Log("Equip Item error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.UseSpiritPills, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                NetworkManager.This.AskCurrencies();
                NetworkManager.This.AskSoulCurrentDatas();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowAlert(message.Json);
                });

                // TODO : Tutorial
                //Application.Current.Properties["AiosKingdom_TutorialStep"] = 4;
                //Application.Current.SavePropertiesAsync();
                //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
            }
            else
            {
                Debug.Log("Use Spirit Pills error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.LearnSkill, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                NetworkManager.This.AskCurrencies();
                NetworkManager.This.AskKnowledges();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowAlert(message.Json);
                });

                // TODO : Tutorial
                //Application.Current.Properties["AiosKingdom_TutorialStep"] = 3;
                //Application.Current.SavePropertiesAsync();
                //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
            }
            else
            {
                Debug.Log("Learn Skill error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.LearnTalent, (message) =>
        {
            SceneLoom.Loom.QueueOnMainThread(() =>
            {
                UIManager.This.HideLoading();
            });

            if (message.Success)
            {
                NetworkManager.This.AskKnowledges();

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    UIManager.This.ShowAlert(message.Json);
                });

                // TODO : tutorial
                //Application.Current.Properties["AiosKingdom_TutorialStep"] = 3;
                //Application.Current.SavePropertiesAsync();
                //MessagingCenter.Send(this, MessengerCodes.TutorialChanged);
            }
            else
            {
                Debug.Log("Learn Talent error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Player.Currencies, (message) =>
        {
            if (message.Success)
            {
                var currencies = JsonConvert.DeserializeObject<JsonObjects.Currencies>(message.Json);

                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    _updateCurrencies(currencies);
                });
            }
            else
            {
                Debug.Log("Currencies error : " + message.Json);
            }
        });

        NetworkManager.This.AddCallback(JsonObjects.CommandCodes.Server.DisconnectSoul, (message) =>
        {
            if (message.Success)
            {
                SceneLoom.Loom.QueueOnMainThread(() =>
                {
                    gameObject.SetActive(false);
                });

                NetworkManager.This.DisconnectGame();
                NetworkManager.This.AskServerList();
            }
        });
    }

    private void _updatePlayerDatas(JsonObjects.SoulDatas datas)
    {
        Level.text = datas.Level.ToString();
        Name.text = datas.Name;

        Experience.text = string.Format(": [{0} / {1}]", datas.CurrentExperience, datas.RequiredExperience);

        Health.text = string.Format(": [{0}]", datas.MaxHealth);
        Mana.text = string.Format(": [{0}]", datas.MaxMana);
        Armor.text = string.Format(": [{0}]", datas.Armor);
        MagicArmor.text = string.Format(": [{0}]", datas.MagicArmor);
        ItemLevel.text = string.Format(": [{0}]", datas.ItemLevel);

        Stamina.text = string.Format(": [{0}]", datas.TotalStamina);
        Energy.text = string.Format(": [{0}]", datas.TotalEnergy);
        Strength.text = string.Format(": [{0}]", datas.TotalStrength);
        Agility.text = string.Format(": [{0}]", datas.TotalAgility);
        Intelligence.text = string.Format(": [{0}]", datas.TotalIntelligence);
        Wisdom.text = string.Format(": [{0}]", datas.TotalWisdom);

        MinDamages.text = string.Format(": [{0}]", datas.MinDamages);
        MaxDamages.text = string.Format(": [{0}]", datas.MaxDamages);
    }

    private void _updateCurrencies(JsonObjects.Currencies currencies)
    {
        Spirits.text = string.Format(": [{0}]", currencies.Spirits);
        Embers.text = string.Format(": [{0}]", currencies.Embers);
        Bits.text = string.Format(": [{0}]", currencies.Bits);
        Shards.text = string.Format(": [{0}]", currencies.Shards);

        PointsAvailable.gameObject.SetActive(false);
        if (currencies.Spirits > 0)
        {
            PointsAvailable.gameObject.SetActive(true);
        }
    }
}
