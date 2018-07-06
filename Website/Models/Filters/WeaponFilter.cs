using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataModels.Items;

namespace Website.Models.Filters
{
    public class WeaponFilter : AItemFilterModel<DataModels.Items.Weapon>
    {
        [Display(Name = "HandlingType")]
        public DataModels.Items.HandlingType? HandlingType { get; set; }

        [Display(Name = "Type")]
        public DataModels.Items.WeaponType? Type { get; set; }

        public override List<Weapon> FilterList(List<Weapon> list)
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

            if (HandlingType != null)
            {
                list = list.Where(a => a.HandlingType.Equals(HandlingType)).ToList();
            }

            if (Type != null)
            {
                list = list.Where(a => a.Type.Equals(Type)).ToList();
            }

            return list;
        }
    }
}