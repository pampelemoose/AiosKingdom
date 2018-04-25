using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Website.Authentication
{
    public class CustomMembershipUser : MembershipUser
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public List<DataModels.Role> Roles { get; set; }

        public CustomMembershipUser(DataModels.User user)
            : base("CustomMembership", user.Username, user.Id,
                  user.Email, string.Empty, string.Empty, true, false, 
                  DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now)
        {
            UserId = user.Id;
            Username = user.Username;
            Roles = user.Roles;
        }
    }
}