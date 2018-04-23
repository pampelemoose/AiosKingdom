using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MinLength(4)]
        public string Username { get; set; }

        [Required, MinLength(8), MaxLength(25)]
        public string Password { get; set; }
    }
}
