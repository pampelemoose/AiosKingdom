using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Authentication
{
    public class CustomSerializeModel
    {
            public Guid Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
        public List<string> Rolenames { get; set; }
    }
}