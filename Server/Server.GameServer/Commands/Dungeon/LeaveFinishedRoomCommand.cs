using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands.Dungeon
{
    public class LeaveFinishedRoomCommand : ACommand
    {
        private DataModels.Config _config;

        public LeaveFinishedRoomCommand(CommandArgs args, DataModels.Config config)
            : base(args)
        {
            _config = config;
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var soul = SoulManager.Instance.GetSoul(_args.ClientId);
            var soulDatas = SoulManager.Instance.GetDatas(_args.ClientId);
            var adventure = AdventureManager.Instance.GetAdventure(soul);

            if (adventure.IsCleared)
            {
                var adventureState = adventure.GetActualState();
                var dungeon = DataRepositories.DungeonRepository.GetById(adventure.DungeonId);

                if (adventureState.IsExit)
                {
                    soul.CurrentExperience += adventureState.StackedExperience;
                    soul.Shards += adventureState.StackedShards;
                    soul.CurrentExperience += dungeon.ExperienceReward;
                    soul.Shards += dungeon.ShardReward;
                }

                while (soul.CurrentExperience >= soulDatas.RequiredExperience)
                {
                    ++soul.Level;

                    soul.Spirits += _config.SpiritsPerLevelUp;
                    soul.Embers += _config.EmbersPerLevelUp;

                    Log.Instance.Write(Log.Level.Infos, $"{soul.Name} Level up {soul.CurrentExperience}/{soulDatas.RequiredExperience} => ({soul.Level}).");

                    soul.CurrentExperience -= soulDatas.RequiredExperience;

                    SoulManager.Instance.UpdateSoul(_args.ClientId, soul);
                    SoulManager.Instance.UpdateCurrentDatas(_args.ClientId, _config);
                    soulDatas = SoulManager.Instance.GetDatas(_args.ClientId);
                    Console.WriteLine($"{soul.Name} Level up({soul.Level}).");
                }

                SoulManager.Instance.UpdateSoul(_args.ClientId, soul);
                SoulManager.Instance.UpdateCurrentDatas(_args.ClientId, _config);
                AdventureManager.Instance.ExitRoom(soul.Id);

                ret.ClientResponse = new Network.Message
                {
                    Code = Network.CommandCodes.Dungeon.LeaveFinishedRoom,
                    Success = true,
                    Json = "Exited the dungeon."
                };
                ret.Succeeded = true;

                return ret;
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Dungeon.LeaveFinishedRoom,
                Success = false,
                Json = "Room not cleared, can't exit."
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
