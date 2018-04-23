using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public static class ClientCodes
    {
        public const int Ping = 0;

        public const int Armor_List = 100;
        public const int Consumable_List = 101;
        public const int Book_List = 102;

        public const int User_LogIn_Response = 10;
        public const int User_Infos = 11;

        public const int Player_Connected = 20;
        public const int Player_Disconnected = 21;

        public const int Player_CurrentDatas = 30;

        public const int Player_Equipment = 40;

        public const int Player_Inventory = 60;

        public const int Market_Items_List = 70;
        public const int Market_Item_Bought = 71;
    }
}
