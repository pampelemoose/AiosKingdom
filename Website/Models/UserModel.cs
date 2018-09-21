using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class UserModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MinLength(4), ConcurrencyCheck]
        public string Username { get; set; }

        [Required, EmailAddress, ConcurrencyCheck]
        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }
}