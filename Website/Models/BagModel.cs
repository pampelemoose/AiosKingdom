using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class BagModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

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

        [Required(ErrorMessage = "Quality required")]
        [Display(Name = "Quality")]
        public DataModels.Items.ItemQuality Quality { get; set; }

        [Required(ErrorMessage = "ItemLevel required")]
        [Display(Name = "Item Level")]
        [Range(1, 400, ErrorMessage = "ItemLevel should be higher than 0")]
        public int ItemLevel { get; set; }

        [Required(ErrorMessage = "UseLevelRequired required")]
        [Display(Name = "Level Required")]
        [Range(1, 400)]
        public int UseLevelRequired { get; set; }

        [Required(ErrorMessage = "SlotCount required")]
        [Display(Name = "Slot Count")]
        [Range(1, 400)]
        public int SlotCount { get; set; }

        [Display(Name = "Stats")]
        public List<DataModels.Items.ItemStat> Stats { get; set; }
    }
}