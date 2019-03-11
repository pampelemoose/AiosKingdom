using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class JobFilter
    {
        [Display(Name = "Type")]
        public DataModels.Jobs.JobType? Type { get; set; }
        [Display(Name = "Rank")]
        public DataModels.Jobs.JobRank? Rank { get; set; }

        public List<DataModels.Jobs.Job> Jobs { get; set; }

        public List<DataModels.Jobs.Job> FilterList(List<DataModels.Jobs.Job> list)
        {
            if (Type != null)
            {
                list = list.Where(a => a.Type.Equals(Type)).ToList();
            }

            if (Rank != null)
            {
                list = list.Where(a => a.Rank.Equals(Rank)).ToList();
            }

            return list;
        }
    }
}