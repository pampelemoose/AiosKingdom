using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public static class CommandCodes
    {
        public const int Client_Authenticate = 0;
        public const int Client_ServerList = 1;

        public const int Client_AnnounceGameConnection = 2;

        public const int Client_SoulList = 10;
        public const int Client_CreateSoul = 11;
        public const int Client_ConnectSoul = 12;
        public const int Client_SoulDatas = 13;
        public const int Client_CurrentSoulDatas = 14;

        public const int Client_MarketList = 20;
        public const int Client_BuyMarketItem = 21;
        public const int Client_EquipItem = 22;

        public const int ArmorList = 30;
        public const int ConsumableList = 31;
        public const int BagList = 32;

        public const int Ping = -1;
    }
}
