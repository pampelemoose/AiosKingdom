using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class PageModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "StatCost required")]
        [Display(Name = "StatCost")]
        [Range(1, 10000, ErrorMessage = "StatCost should be higher than 0")]
        public int StatCost { get; set; }

        [Required(ErrorMessage = "Rank required")]
        [Display(Name = "Rank")]
        [Range(1, 400)]
        public int Rank { get; set; }

        [Required(ErrorMessage = "ManaCost required")]
        [Display(Name = "ManaCost")]
        [Range(0, 400)]
        public int ManaCost { get; set; }

        [Required(ErrorMessage = "Cooldown required")]
        [Display(Name = "Cooldown")]
        [Range(0, 400)]
        public int Cooldown { get; set; }

        [Display(Name = "New Insc")]
        [Range(0, 1000000)]
        public int NewInsc { get; set; }

        public List<InscriptionModel> Inscriptions { get; set; }
    }
}