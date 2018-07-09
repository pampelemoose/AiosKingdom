using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class BookFilter
    {
        [Required]
        public Guid SelectedVersion { get; set; }
        [Display(Name = "Version")]
        public List<DataModels.Version> VersionList { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Quality")]
        public DataModels.Skills.BookQuality? Quality { get; set; }

        public List<DataModels.Skills.Book> Books { get; set; }

        public List<DataModels.Skills.Book> FilterList(List<DataModels.Skills.Book> list)
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

            return list;
        }
    }
}