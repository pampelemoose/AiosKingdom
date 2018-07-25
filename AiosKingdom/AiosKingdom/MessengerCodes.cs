using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom
{
    public static class MessengerCodes
    {
        public static string ConnectionFailed = "connectionFailed";
        public static string Disconnected = "disconnected";

        public static string LoginSuccessful = "loginSuccessful";
        public static string LoginFailed = "loginFailed";

        public static string ServerListReceived = "initialDatasReceived";
        public static string GameServerDatasReceived = "gameServerDatasReceived";
        public static string GameServerDisconnected = "gameServerDisconnected";

        public static string ConnectedToServer = "connectedToServer";
        public static string SoulListReceived = "soulListReceived";
        public static string SoulCreationFailed = "soulCreationFailed";

        public static string SoulConnected = "soulConnected";
        public static string SoulConnectionFailed = "soulConnectionFailed";
        public static string SoulUpdated = "soulUpdated";
        public static string SoulDatasUpdated = "soulDatasUpdated";

        public static string MarketUpdated = "marketUpdated";

        public static string SpiritPillUsed = "spiritPillUsed";
        public static string SkillLearned = "skillLearned";

        public static string EnterDungeon = "enterDungeon";
        public static string EnterDungeonFailed = "enterDungeonFailed";
        public static string DungeonSelectSkillEnded = "dungeonSelectSkillEnded";
        public static string DungeonSelectConsumableEnded = "dungeonSelectConsumableEnded";
        public static string EnemyTurnEnded = "enemyTurnEnded";
        public static string DungeonUpdated = "dungeonUpdated";
        public static string DungeonLootsReceived = "dungeonLootsReceived";

        public static string OpenLoadingScreen = "openLoadingScreen";
        public static string LoadingScreenOpenned = "loadingScreenOpenned";
        public static string UpdateLoadingScreen = "updateLoadingScreen";
        public static string AlertLoadingScreen = "alertLoadingScreen";
        public static string CloseLoadingScreen = "closeLoadingScreen";
        public static string LoadingScreenClosed = "loadingScreenClosed";
        public static string LoadingScreenChangePage = "loadingScreenChangePage";
        public static string LoadingScreenPushPage = "loadingScreenPushPage";
    }
}
