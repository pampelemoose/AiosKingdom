using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class UserFilter
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "IsActive")]
        public bool? IsActive { get; set; }

        public List<DataModels.User> Users { get; set; }

        public List<DataModels.User> FilterList(List<DataModels.User> list)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Username.Contains(Name)).ToList();
            }

            if (!string.IsNullOrEmpty(Email))
            {
                list = list.Where(a => a.Email.Contains(Email)).ToList();
            }

            if (IsActive != null)
            {
                list = list.Where(a => a.IsActivated.Equals(IsActive)).ToList();
            }

            return list;
        }
    }
}