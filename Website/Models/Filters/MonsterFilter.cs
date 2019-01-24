using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class MonsterFilter
    {
        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        public List<DataModels.Monsters.Monster> Monsters { get; set; }

        public List<DataModels.Monsters.Monster> FilterList(List<DataModels.Monsters.Monster> list)
        {
            if (!Guid.Empty.Equals(SelectedVersion))
            {
                list = list.Where(a => a.VersionId.Equals(SelectedVersion)).ToList();
            }

            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }

            return list;
        }
    }
}