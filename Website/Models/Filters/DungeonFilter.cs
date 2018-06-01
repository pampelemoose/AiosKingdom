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
        public Guid SelectedVersion { get; set; }
        [Display(Name = "Version")]
        public List<DataModels.Version> VersionList { get; set; }

        public List<DataModels.Dungeons.Dungeon> Dungeons { get; set; }

        public List<DataModels.Dungeons.Dungeon> FilterList(List<DataModels.Dungeons.Dungeon> list)
        {
            if (!Guid.Empty.Equals(SelectedVersion))
            {
                list = list.Where(a => a.VersionId.Equals(SelectedVersion)).ToList();
            }

            return list;
        }
    }
}