using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdventureListItem : MonoBehaviour
{
    public NetworkManager Network;

    public Text Name;
    public Text MinLevel;
    public Text MaxLevel;

    public void SetDatas(JsonObjects.Adventures.Dungeon dungeon)
    {
        Name.text = dungeon.Name;
        MinLevel.text = dungeon.RequiredLevel.ToString();
        MaxLevel.text = dungeon.MaxLevelAuthorized.ToString();

        //ConnectButton.onClick.AddListener(() =>
        //{
        //    LoadingScreen.Loading.Show();

        //    Network.AnnounceGameServerConnection(infos.Id);
        //});
    }
}
