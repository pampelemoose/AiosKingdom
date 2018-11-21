using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class InscWeaponTypeModel
    {
        public string PageId { get; set; }
        public string InscId { get; set; }
        public string TypeExtension { get; set; }

        public DataModels.Items.ItemType? Type { get; set; }
    }
}