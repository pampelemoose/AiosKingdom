using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels
{
    public class AuthToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid Token { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
