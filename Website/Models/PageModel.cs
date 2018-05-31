using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class PageModel
    {
        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        private string _image = "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";
        [Display(Name = "Image")]
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

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

        public List<DataModels.Skills.Inscription> Inscriptions { get; set; }
    }
}