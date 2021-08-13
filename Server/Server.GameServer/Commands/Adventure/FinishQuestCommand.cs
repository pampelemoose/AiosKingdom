using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Adventure
{
    public class FinishQuestCommand : ACommand
    {
        public FinishQuestCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var questId = Guid.Parse(_args.Args[0]);

            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.FinishQuest,
                    Success = false,
                    Json = "No adventure found."
                };
                ret.Succeeded = true;

                return ret;
            }

            var quests = adventure.GetQuests();
            var quest = quests.FirstOrDefault(q => q.QuestId == questId);

            if (quest == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.FinishQuest,
                    Success = false,
                    Json = "No quest found."
                };
                ret.Succeeded = true;

                return ret;
            }

            bool isFinished = true;
            foreach (var objective in quest.Objectives)
            {
                if (!objective.Finished)
                {
                    isFinished = false;
                }
            }

            if (!isFinished)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.FinishQuest,
                    Success = false,
                    Json = "Quest not finished."
                };
                ret.Succeeded = true;

                return ret;
            }

            adventure.FinishQuest(questId);

            var state = adventure.GetActualState();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Adventure.FinishQuest,
                Success = true,
                Json = questId.ToString()
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
