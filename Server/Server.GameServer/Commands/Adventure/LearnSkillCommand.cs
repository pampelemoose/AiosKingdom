using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Adventure
{
    public class LearnSkillCommand : ACommand
    {
        public LearnSkillCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);

            var bookId = Guid.Parse(_args.Args[0]);

            var knowledges = SoulManager.Instance.GetKnowledges(ret.ClientId);
            var data = SoulManager.Instance.GetBaseDatas(ret.ClientId);

            var alreadyLearned = knowledges.FirstOrDefault(k => k.BookId.Equals(bookId));
            if (alreadyLearned != null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.LearnSkill,
                    Success = false,
                    Json = "Skill already known."
                };
                ret.Succeeded = true;
                return ret;
            }

            var skill = DataManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(bookId));
            if (skill == null)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.LearnSkill,
                    Success = false,
                    Json = "Skill rank doesn't exists."
                };
                ret.Succeeded = true;
                return ret;
            }

            if (skill.ExperienceCost > data.CurrentExperience)
            {
                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Adventure.LearnSkill,
                    Success = false,
                    Json = "Not enough experience."
                };
                ret.Succeeded = true;
                return ret;
            }

            data.CurrentExperience -= skill.ExperienceCost;

            knowledges.Add(new Network.Knowledge
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                TalentPoints = 0,
                Talents = new List<Network.TalentUnlocked>(),
                IsNew = true
            });

            var adventure = AdventureManager.Instance.GetAdventure(soulId);
            var soulData = SoulManager.Instance.GetDatas(ret.ClientId);
            adventure.UpdateKnowledges(soulData, knowledges);

            // QUEST
            var quests = adventure.GetQuests();
            bool objectiveUpdated = false;
            foreach (var questState in quests)
            {
                if (!questState.Finished)
                {
                    var quest = DataManager.Instance.Quests.FirstOrDefault(q => q.Id == questState.QuestId);

                    if (quest == null)
                    {
                        continue;
                    }

                    foreach (var objective in quest.Objectives)
                    {
                        if (objective.Type == Network.Adventures.ObjectiveType.LearnBook)
                        {
                            var objectiveState = questState.Objectives.FirstOrDefault(o => o.ObjectiveId == objective.Id);

                            if (objectiveState == null)
                            {
                                continue;
                            }

                            var objectiveContent = JsonConvert.DeserializeObject<DataModels.Adventures.QuestObjectiveDataLearnBook>(objective.DataContent);

                            if (!objectiveContent.Books.Contains(bookId))
                            {
                                continue;
                            }

                            ++objectiveState.Quantity;

                            if (objectiveState.Quantity == objectiveContent.NeedToLearnCount)
                            {
                                objectiveState.Finished = true;
                            }

                            adventure.UpdateQuest(questState);
                            objectiveUpdated = true;
                            break;
                        }
                    }

                    if (objectiveUpdated)
                    {
                        break;
                    }
                }
            }

            SoulManager.Instance.UpdateBaseDatas(ret.ClientId, data);
            SoulManager.Instance.UpdateKnowledge(ret.ClientId, knowledges);

            var adventureState = adventure.GetActualState();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Adventure.LearnSkill,
                Success = true,
                Json = JsonConvert.SerializeObject(adventureState)
            };
            ret.Succeeded = true;
            return ret;
        }
    }
}
