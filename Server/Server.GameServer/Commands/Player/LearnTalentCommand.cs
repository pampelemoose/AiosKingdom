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
            var talents = DataManager.Instance.Books.SelectMany(b => b.Talents);
            var unlocks = knowledges.SelectMany(k => k.Talents);
            var talent = talents.FirstOrDefault(t => t.Id.Equals(talentId));
            var talentsAtLeaf = talents.Where(t => t.Branch == talent.Branch && t.Leaf == talent.Leaf);
            var talentAtLeafIds = talentsAtLeaf.Select(t => t.Id);
            var unlock = unlocks.FirstOrDefault(u => talentAtLeafIds.Contains(u.TalentId));

            if (talent != null)
            {
                var knowledge = knowledges.FirstOrDefault(k => k.BookId.Equals(talent.BookId));

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
                        // CHANGE TALENT
                        var lUnlock = knowledge.Talents.FirstOrDefault(t => t.TalentId.Equals(talent.Id));
                        knowledge.Talents.Remove(lUnlock);

                        lUnlock.TalentId = talent.Id;
                        lUnlock.UnlockedAt = DateTime.Now;

                        knowledge.Talents.Add(lUnlock);
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
