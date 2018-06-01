﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class DungeonModel
    {
        [Required]
        public Guid SelectedVersion { get; set; }
        [Display(Name = "Version")]
        public List<DataModels.Version> VersionList { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        /*private string _image = "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";
        [Display(Name = "Image")]
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }*/

        [Required(ErrorMessage = "RequiredLevel required")]
        [Display(Name = "RequiredLevel")]
        [Range(1, 10000, ErrorMessage = "RequiredLevel should be higher than 0")]
        public int RequiredLevel { get; set; }

        [Required(ErrorMessage = "MaxLevelAuthorized required")]
        [Display(Name = "MaxLevelAuthorized")]
        [Range(1, 10000, ErrorMessage = "MaxLevelAuthorized should be higher than 0")]
        public int MaxLevelAuthorized { get; set; }

        [Display(Name = "Rooms")]
        public List<RoomModel> Rooms { get; set; }
    }
}