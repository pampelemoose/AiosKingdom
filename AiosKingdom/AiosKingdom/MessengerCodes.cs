using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom
{
    public static class MessengerCodes
    {
        public static string ConnectionFailed = "connectionFailed";
        public static string Disconnected = "disconnected";

        public static string CreateNewAccount = "createNewAccount";
        public static string CreateNewAccountFailed = "createNewAccountFailed";
        public static string RetrievedAccount = "retrievedAccount";

        public static string LoginSuccessful = "loginSuccessful";
        public static string LoginFailed = "loginFailed";

        public static string ServerListReceived = "initialDatasReceived";
        public static string GameServerDisconnected = "gameServerDisconnected";

        public static string ConnectedToServerFailed = "connectedToServerFailed";

        public static string SoulListReceived = "soulListReceived";
        public static string CreateSoulFailed = "createSoulFailed";
        public static string CreateSoulSuccess = "createSoulSuccess";

        public static string SoulConnected = "soulConnected";
        public static string SoulDatasUpdated = "soulDatasUpdated";

        public static string CurrenciesUpdated = "currenciesUpdated";
        public static string InventoryUpdated = "inventoryUpdated";
        public static string KnowledgeUpdated = "knowledgeUpdated";
        public static string EquipmentUpdated = "equipmentUpdated";

        public static string MarketUpdated = "marketUpdated";
        public static string BuyMarketItem = "buyMarketItem";
        
        public static string SpiritPillsFailed = "spiritPillsFailed";
        public static string SkillLearned = "skillLearned";

        public static string EnterDungeon = "enterDungeon";
        public static string ExitedDungeon = "exitedDungeon";
        public static string DungeonSelectSkillEnded = "dungeonSelectSkillEnded";
        public static string DungeonSelectConsumableEnded = "dungeonSelectConsumableEnded";
        public static string EnemyTurnEnded = "enemyTurnEnded";
        public static string DungeonUpdated = "dungeonUpdated";
        public static string DungeonLootsReceived = "dungeonLootsReceived";

        public static string AlertScreen = "alertScreen";
        public static string ScreenChangePage = "screenChangePage";
        public static string ScreenPushPage = "screenPushPage";

        public static string TutorialChanged = "tutorialChanged";
    }
}
