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

            var alreadyLearned = knowledges.FirstOrDefault(k => k.Id.Equals(bookId));
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

            // QUEST
            var adventure = AdventureManager.Instance.GetAdventure(soulId);
            var quests = adventure.GetQuests();
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
                            break;
                        }
                    }
                }
            }

            /*
            if (rank == 1)
            {
                var alreadyLearned = knowledges.FirstOrDefault(k => k.Id.Equals(bookId) && k.Rank.Equals(rank));
                if (alreadyLearned != null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.LearnSkill,
                        Success = false,
                        Json = "Skill already known."
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                var skill = DataManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(bookId)).Pages.FirstOrDefault(p => p.Rank.Equals(rank));
                if (skill == null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.LearnSkill,
                        Success = false,
                        Json = "Skill rank doesn't exists."
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                if (skill.EmberCost > currencies.Embers)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.LearnSkill,
                        Success = false,
                        Json = "Not enough embers."
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                currencies.Embers -= skill.EmberCost;

                knowledges.Add(new Network.Knowledge
                {
                    Id = Guid.NewGuid(),
                    BookId = bookId,
                    Rank = rank,
                    IsNew = true
                });
            }
            else
            {
                var hasRank = knowledges.FirstOrDefault(k => k.BookId.Equals(bookId) && k.Rank.Equals(rank - 1));
                if (hasRank == null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.LearnSkill,
                        Success = false,
                        Json = "Don't have previous rank, can't upgrade."
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                var skill = DataManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(bookId)).Pages.FirstOrDefault(p => p.Rank.Equals(rank));
                if (skill == null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.LearnSkill,
                        Success = false,
                        Json = "Skill rank doesn't exists."
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                if (skill.EmberCost > currencies.Embers)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.LearnSkill,
                        Success = false,
                        Json = "Not enough embers."
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                currencies.Embers -= skill.EmberCost;

                knowledges.Remove(hasRank);
                hasRank.Rank = rank;
                knowledges.Add(hasRank);

            }
            */

            SoulManager.Instance.UpdateBaseDatas(ret.ClientId, data);
            SoulManager.Instance.UpdateKnowledge(ret.ClientId, knowledges);

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Adventure.LearnSkill,
                Success = true,
                Json = "Skill learned."
            };
            ret.Succeeded = true;
            return ret;
        }
    }
}
