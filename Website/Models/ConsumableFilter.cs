using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class ConsumableFilter : AItemFilterModel<DataModels.Items.Consumable>
    {
        public override List<DataModels.Items.Consumable> FilterList(List<DataModels.Items.Consumable> list)
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

            return list;
        }
    }
}