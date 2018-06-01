using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class BagFilter : AItemFilterModel<DataModels.Items.Bag>
    {
        [Display(Name = "SlotCount")]
        public int SlotCount { get; set; }

        public override List<DataModels.Items.Bag> FilterList(List<DataModels.Items.Bag> list)
        {
            if (!Guid.Empty.Equals(SelectedVersion))
            {
                list = list.Where(a => a.VersionId.Equals(SelectedVersion)).ToList();
            }

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

            if (SlotCount > 0)
            {
                list = list.Where(a => a.SlotCount.Equals(SlotCount)).ToList();
            }

            return list;
        }
    }
}