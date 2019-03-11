using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class RecipeFilter
    {
        [Display(Name = "Type")]
        public DataModels.Jobs.JobType? Type { get; set; }

        public List<DataModels.Jobs.Recipe> Recipes { get; set; }

        public List<DataModels.Jobs.Recipe> FilterList(List<DataModels.Jobs.Recipe> list)
        {
            if (Type != null)
            {
                list = list.Where(a => a.JobType.Equals(Type)).ToList();
            }

            return list;
        }
    }
}