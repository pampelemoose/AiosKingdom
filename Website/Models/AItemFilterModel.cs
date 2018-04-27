using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public abstract class AItemFilterModel<T>
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Quality")]
        public DataModels.Items.ItemQuality? Quality { get; set; }
        [Display(Name = "ItemLevel")]
        public int ItemLevel { get; set; }
        [Display(Name = "LevelRequired")]
        public int UseLevelRequired { get; set; }

        public List<T> Items { get; set; }

        public abstract List<T> FilterList(List<T> list);
    }
}