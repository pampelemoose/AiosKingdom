using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects
{
    public static class CommandCodes
    {
        public const int Ping = -1;

        public const int Client_CreateAccount = 0;
        public const int Client_Authenticate = 1;
        public const int Client_ServerList = 2;
        public const int Client_AnnounceGameConnection = 3;
        public const int Client_AnnounceDisconnection = 4;
        public const int Client_RetrieveAccount = 5;

        public const int GlobalMessage = 1337;

        public static class Server
        {
            private const int _default = 10;

            public const int SoulList = _default + 1;
            public const int CreateSoul = _default + 2;
            public const int ConnectSoul = _default + 3;
            public const int DisconnectSoul = _default + 4;
        }

        public static class Player
        {
            private const int _default = 100;

            public const int Market_PlaceOrder = _default + 1;
            public const int Market_OrderProcessed = _default + 12;
            public const int EquipItem = _default + 2;
            public const int SellItem = _default + 11;
            public const int UseSpiritPills = _default + 3;
            public const int LearnSkill = _default + 4;
            public const int LearnTalent = _default + 14;

            public const int CurrentSoulDatas = _default + 6;

            public const int Currencies = _default + 7;
            public const int Inventory = _default + 8;
            public const int Knowledges = _default + 9;
            public const int Equipment = _default + 10;
            public const int AdventureUnlocked = _default + 13;
        }

        public static class Listing
        {
            private const int _default = 30;

            public const int Item = _default + 1;
            public const int Book = _default + 2;
            public const int Monster = _default + 3;
            public const int Dungeon = _default + 4;
            public const int Market = _default + 5;
            public const int SpecialsMarket = _default + 6;
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
            public const int PlayerDied = _default + 13;
            public const int PlayerRest = _default + 14;
        }
    }
}
