﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class BookModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Quality required")]
        [Display(Name = "Quality")]
        public DataModels.Skills.BookQuality Quality { get; set; }

        [Display(Name = "Pages")]
        public List<PageModel> Pages { get; set; }
    }
}