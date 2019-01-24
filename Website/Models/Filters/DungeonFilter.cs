using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class DungeonFilter
    {
        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "RequiredLevel")]
        public int RequiredLevel { get; set; }

        [Display(Name = "MaxLevel")]
        public int MaxLevel { get; set; }

        public List<DataModels.Adventures.Adventure> Dungeons { get; set; }

        public List<DataModels.Adventures.Adventure> FilterList(List<DataModels.Adventures.Adventure> list)
        {
            if (!Guid.Empty.Equals(SelectedVersion))
            {
                list = list.Where(a => a.VersionId.Equals(SelectedVersion)).ToList();
            }

            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }

            if (RequiredLevel > 0)
            {
                list = list.Where(a => a.RequiredLevel.Equals(RequiredLevel)).ToList();
            }

            if (MaxLevel > 0)
            {
                list = list.Where(a => a.MaxLevelAuthorized.Equals(MaxLevel)).ToList();
            }

            return list;
        }
    }
}