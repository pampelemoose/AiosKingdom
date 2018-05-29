﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class ClientLearnSkillCommand : ACommand
    {
        public ClientLearnSkillCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var bookId = Guid.Parse(_args.Args[0]);
            var rank = int.Parse(_args.Args[1]);

            var datas = SoulManager.Instance.GetSoul(ret.ClientId);

            if (rank == 1)
            {
                var alreadyLearned = datas.Knowledge.FirstOrDefault(k => k.BookId.Equals(bookId) && k.Rank.Equals(rank));
                if (alreadyLearned != null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_LearnSkill,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Skill already known." })
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                var skill = DataRepositories.BookRepository.GetById(bookId).Pages.FirstOrDefault(p => p.Rank.Equals(rank));
                if (skill == null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_LearnSkill,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Skill rank doesn't exists." })
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                if (skill.EmberCost > datas.Embers)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_LearnSkill,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Not enough embers." })
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                datas.Embers -= skill.EmberCost;

                datas.Knowledge.Add(new DataModels.Knowledge
                {
                    SoulId = datas.Id,
                    BookId = bookId,
                    Rank = rank
                });

                DataRepositories.SoulRepository.Update(datas);
            }
            else
            {
                var hasRank = datas.Knowledge.FirstOrDefault(k => k.BookId.Equals(bookId) && k.Rank.Equals(rank - 1));
                if (hasRank == null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_LearnSkill,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Don't have previous rank, can't upgrade." })
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                var skill = DataRepositories.BookRepository.GetById(bookId).Pages.FirstOrDefault(p => p.Rank.Equals(rank));
                if (skill == null)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_LearnSkill,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Skill rank doesn't exists." })
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                if (skill.EmberCost > datas.Embers)
                {
                    ret.ClientResponse = new Network.Message
                    {
                        Code = Network.CommandCodes.Client_LearnSkill,
                        Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = false, Message = "Not enough embers." })
                    };
                    ret.Succeeded = true;
                    return ret;
                }

                datas.Embers -= skill.EmberCost;

                datas.Knowledge.Remove(hasRank);
                hasRank.Rank = rank;
                datas.Knowledge.Add(hasRank);

                DataRepositories.SoulRepository.Update(datas);
            }

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.Client_LearnSkill,
                Json = JsonConvert.SerializeObject(new Network.MessageResult { Success = true, Message = "Skill learned." })
            };
            ret.Succeeded = true;
            return ret;
        }
    }
}