using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class ArmorFilter
    {
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Quality")]
        public DataModels.Items.ItemQuality? Quality { get; set; }
        [Display(Name = "ItemLevel")]
        public int ItemLevel { get; set; }
        [Display(Name = "LevelRequired")]
        public int UseLevelRequired { get; set; }
        [Display(Name = "Part")]
        public DataModels.Items.ArmorPart? Part { get; set; }

        public List<DataModels.Items.Armor> Armors { get; set; }

        public List<DataModels.Items.Armor> FilterList(List<DataModels.Items.Armor> list)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }

            if (Quality != null)
            {
                list = list.Where(a => a.Quality.Equals(Quality)).ToList();
            }

            if (ItemLevel > 0)
            {
                list = list.Where(a => a.ItemLevel.Equals(ItemLevel)).ToList();
            }

            if (UseLevelRequired > 0)
            {
                list = list.Where(a => a.UseLevelRequired.Equals(UseLevelRequired)).ToList();
            }

            if (Part != null)
            {
                list = list.Where(a => a.Part.Equals(Part)).ToList();
            }

            return list;
        }
    }
}