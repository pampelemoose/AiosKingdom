using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public static class ServerCodes
    {
        public const int Pong = 0;
        public const int Help_Command = 1;

        public const int User_Create_Command = 11;
        public const int User_Delete_Command = 12;
        public const int User_Infos_Command = 13;
        public const int User_Login_Command = 14;

        public const int Soul_Create_Command = 21;
        public const int Soul_Delete_Command = 22;
        public const int Soul_Infos_Command = 23;

        public const int Player_Connect_Command = 101;
        public const int Player_Disconnect_Command = 102;
        public const int Player_CurrentDatas_Command = 105;
        public const int Player_GiveExperience_Command = 106;
        public const int Player_UsePills_Command = 107;
        public const int Player_LootItem_Command = 141;
        public const int Player_DestroyItem_Command = 142;

        public const int Equipment_List_Command = 120;
        public const int Equipment_EquipItem_Command = 121;

        public const int Inventory_List_Command = 140;


        public const int Item_Consumable_List_Command = 200;
        public const int Item_Consumable_Infos_Command = 203;

        public const int Item_Armor_List_Command = 210;
        public const int Item_Armor_Infos_Command = 213;

        public const int Book_List_Command = 220;
        public const int Book_Infos_Command = 221;

        public const int Market_Item_List_Command = 300;
        public const int Market_Buy_Item_Command = 301;
    }
}
