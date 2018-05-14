using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom
{
    public static class MessengerCodes
    {
        public static string ConnectionSuccessful = "connectionSuccessful";
        public static string ConnectionFailed = "connectionFailed";
        public static string Disconnected = "disconnected";

        public static string LoginSuccessful = "loginSuccessful";
        public static string LoginFailed = "loginFailed";

        public static string ServerListReceived = "initialDatasReceived";
        public static string GameServerDatasReceived = "gameServerDatasReceived";
        public static string GameServerDisconnected = "gameServerDisconnected";

        public static string ShowSoulList = "showSoulList";
        public static string SoulListReceived = "soulListReceived";
        public static string SoulCreationFailed = "soulCreationFailed";

        public static string SoulConnected = "soulConnected";
        public static string SoulConnectionFailed = "soulConnectionFailed";
        public static string SoulUpdated = "soulUpdated";
        public static string SoulDatasUpdated = "soulDatasUpdated";

        public static string ArmorListUpdated = "armorListUpdated";
        public static string InventoryUpdated = "inventoryUpdated";
        public static string MarketUpdated = "marketUpdated";
    }
}
