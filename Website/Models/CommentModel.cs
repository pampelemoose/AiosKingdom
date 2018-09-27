using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class CommentModel
    {
        [Required]
        [MinLength(20, ErrorMessage = "Message should be at least 20 characters.")]
        public string Content { get; set; }
    }
}