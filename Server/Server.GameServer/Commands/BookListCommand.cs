using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer.Commands
{
    public class BookListCommand : ACommand
    {
        public BookListCommand(CommandArgs args) 
            : base(args)
        {
        }

        protected override CommandResult ExecuteLogic(CommandResult ret)
        {
            var books = DataRepositories.BookRepository.GetAll();

            ret.ClientResponse = new Network.Message
            {
                Code = Network.CommandCodes.BookList,
                Json = JsonConvert.SerializeObject(books)
            };
            ret.Succeeded = true;

            return ret;
        }
    }
}
