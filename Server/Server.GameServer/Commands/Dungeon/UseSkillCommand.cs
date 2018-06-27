using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class UseSkillCommand : ACommand
    {
        public UseSkillCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var knowledgeId = Guid.Parse(_args.Args[0]);
            var enemyId = Guid.Parse(_args.Args[1]);

            var adventure = AdventureManager.Instance.GetAdventure(soul);

            if (adventure != null)
            {
                var skillKnown = soul.Knowledge.FirstOrDefault(s => s.Id.Equals(knowledgeId));
                if (skillKnown != null)
                {
                    var skill = DataRepositories.BookRepository.GetById(skillKnown.BookId).Pages.FirstOrDefault(p => p.Rank.Equals(skillKnown.Rank));
                    if (skill != null)
                    {
                        var datas = SoulManager.Instance.GetDatas(ret.ClientId);

                        if (adventure.UseSkill(skill, SoulManager.Instance.GetDatas(ret.ClientId), enemyId))
                        {
                            ret.ClientResponse = new Network.Message
                            {
                                Code = Network.CommandCodes.Dungeon.UseSkill,
                                Success = true,
                                Json = "Skill successfully used."
                            };
                            ret.Succeeded = true;

                            return ret;
                        }
                    }
                }
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.UseSkill,
                Success = false,
                Json = "Failed to use skill."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
