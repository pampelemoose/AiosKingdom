using Newtonsoft.Json;
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
            var soulId = SoulManager.Instance.GetSoulId(_args.ClientId);
            var knowledges = SoulManager.Instance.GetKnowledges(_args.ClientId);
            var knowledgeId = Guid.Parse(_args.Args[0]);
            var enemyId = Guid.Parse(_args.Args[1]);

            var adventure = AdventureManager.Instance.GetAdventure(soulId);

            if (adventure != null)
            {
                var skillKnown = knowledges.FirstOrDefault(s => s.Id.Equals(knowledgeId));
                if (skillKnown != null)
                {
                    var skill = DataManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(skillKnown.BookId));
                    if (skill != null)
                    {
                        var datas = SoulManager.Instance.GetDatas(ret.ClientId);

                        List<Network.AdventureState.ActionResult> skillResult;
                        if (adventure.UseSkill(skill, enemyId, out skillResult))
                        {
                            var state = adventure.GetActualState();

                            if (state.State.CurrentHealth <= 0)
                            {
                                AdventureManager.Instance.PlayerDied(soulId);

                                skillResult.Add(new Network.AdventureState.ActionResult
                                {
                                    ResultType = Network.AdventureState.ActionResult.Type.PlayerDeath
                                });

                                ret.ClientResponse = new Network.Message
                                {
                                    Code = Network.CommandCodes.Dungeon.PlayerDied,
                                    Success = true,
                                    Json = JsonConvert.SerializeObject(skillResult)
                                };
                                ret.Succeeded = true;
                            }
                            else
                            {
                                ret.ClientResponse = new Network.Message
                                {
                                    Code = Network.CommandCodes.Dungeon.UseSkill,
                                    Success = true,
                                    Json = JsonConvert.SerializeObject(skillResult)
                                };
                                ret.Succeeded = true;
                            }

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
