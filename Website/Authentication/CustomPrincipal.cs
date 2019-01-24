using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Website.Authentication
{
    public class CustomPrincipal : IPrincipal
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }

        public IIdentity Identity { get; }

        public bool IsInRole(string role)
        {
            if (Roles != null && Roles.Any(r => role.Contains(r)))
            {
                return true;
            }

            return false;
        }

        public CustomPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
        }
    }
}