using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public static class CommandCodes
    {
        public const int Ping = -1;

        public const int Client_Authenticate = 0;
        public const int Client_ServerList = 1;
        public const int Client_AnnounceGameConnection = 2;

        public static class Server
        {
            private const int _default = 10;

            public const int SoulList = _default + 1;
            public const int CreateSoul = _default + 2;
            public const int ConnectSoul = _default + 3;
        }

        public static class Player
        {
            private const int _default = 20;

            public const int BuyMarketItem = _default + 1;
            public const int EquipItem = _default + 2;
            public const int UseSpiritPills = _default + 3;
            public const int LearnSkill = _default + 4;

            public const int SoulDatas = _default + 5;
            public const int CurrentSoulDatas = _default + 6;
        }

        public static class Listing
        {
            private const int _default = 30;

            public const int Armor = _default + 1;
            public const int Consumable = _default + 2;
            public const int Bag = _default + 3;
            public const int Book = _default + 4;
            public const int Monster = _default + 5;
            public const int Dungeon = _default + 6;
            public const int Market = _default + 7;
        }

        public static class Dungeon
        {
            private const int _default = 50;

            public const int Enter = _default + 1;
            public const int EnterRoom = _default + 2;
            public const int Exit = _default + 3;
            public const int UpdateRoom = _default + 4;
            public const int UseSkill = _default + 5;
            public const int UseConsumable = _default + 6;
            public const int EnemyTurn = _default + 7;
            public const int GetLoots = _default + 8;
            public const int LootItem = _default + 9;
            public const int LeaveFinishedRoom = _default + 10;
            public const int DoNothingTurn = _default + 11;
            public const int BuyShopItem = _default + 12;
        }
    }
}
