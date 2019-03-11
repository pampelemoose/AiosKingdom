using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Jobs
{
    public class LearnCommand : ACommand
    {
        public LearnCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var job = SoulManager.Instance.GetJob(ret.ClientId);
            var jobType = (Network.JobType)Enum.Parse(typeof(Network.JobType), _args.Args[0]);

            if (job == null)
            {
                job = new Network.Job
                {
                    Points = 0,
                    Rank = Network.JobRank.Apprentice,
                    Type = jobType,
                    Recipes = new List<Network.RecipeUnlocked>()
                };

                SoulManager.Instance.UpdateJob(ret.ClientId, job);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Job.Learn,
                    Json = JsonConvert.SerializeObject(job),
                    Success = true
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Job.Learn,
                Json = "Job already learned.",
                Success = false
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
