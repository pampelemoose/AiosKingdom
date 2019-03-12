using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Player
{
    public class LearnTalentCommand : ACommand
    {
        public LearnTalentCommand(CommandArgs args)
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var talentId = Guid.Parse(_args.Args[0]);
            var knowledges = SoulManager.Instance.GetKnowledges(_args.ClientId);
            var talents = DataManager.Instance.Books.Where(b => knowledges.Select(k => k.BookId).Contains(b.Id)).SelectMany(b => b.Talents);
            var talent = talents.FirstOrDefault(t => t.Id.Equals(talentId));

            if (talent != null)
            {
                var knowledge = knowledges.FirstOrDefault(k => k.BookId.Equals(talent.BookId));
                var unlocks = knowledges.SelectMany(k => k.Talents);
                var talentsAtLeaf = talents.Where(t => t.Branch == talent.Branch && t.Leaf == talent.Leaf);
                var unlock = unlocks.FirstOrDefault(u => talentsAtLeaf.Select(t => t.Id).Contains(u.TalentId));

                if (knowledge.TalentPoints >= talent.TalentPointsRequired)
                {
                    knowledges.Remove(knowledge);

                    if (unlock == null)
                    {
                        // CREATE TALENT
                        knowledge.Talents.Add(new Network.TalentUnlocked
                        {
                            Id = Guid.NewGuid(),
                            KnowledgeId = knowledge.Id,
                            TalentId = talent.Id,
                            IsNew = true,
                            UnlockedAt = DateTime.Now
                        });
                    }
                    else
                    {
                        knowledge.Talents.RemoveAll(t => t.TalentId.Equals(unlock.TalentId));

                        if (unlock.KnowledgeId.Equals(knowledge.Id))
                        {
                            unlock.TalentId = talent.Id;
                            unlock.UnlockedAt = DateTime.Now;

                            knowledge.Talents.Add(unlock);
                        }
                        else
                        {
                            var oldKnowledge = knowledges.FirstOrDefault(k => k.Id.Equals(unlock.KnowledgeId));
                            knowledges.Remove(oldKnowledge);

                            oldKnowledge.Talents.RemoveAll(t => t.TalentId.Equals(unlock.TalentId));
                            knowledges.Add(oldKnowledge);

                            // CREATE TALENT
                            knowledge.Talents.Add(new Network.TalentUnlocked
                            {
                                Id = Guid.NewGuid(),
                                KnowledgeId = knowledge.Id,
                                TalentId = talent.Id,
                                IsNew = true,
                                UnlockedAt = DateTime.Now
                            });
                        }
                    }

                    knowledge.TalentPoints -= talent.TalentPointsRequired;

                    knowledges.Add(knowledge);
                    SoulManager.Instance.UpdateKnowledge(_args.ClientId, knowledges);

                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Player.LearnTalent,
                        Success = true,
                        Json = "Talent Learned."
                    };
                    ret.Succeeded = true;
                    return ret;
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Player.LearnTalent,
                Success = false,
                Json = "Could not learn talent."
            };
            ret.Succeeded = true;
            return ret;
        }
    }
}
