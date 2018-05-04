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
    }
}
