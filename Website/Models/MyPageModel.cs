using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class MyPageModel
    {
        public DataModels.User User { get; set; }

        public List<DataModels.Website.Ticket> Tickets { get; set; }
    }
}