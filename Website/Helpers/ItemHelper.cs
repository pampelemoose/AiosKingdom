using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website.Helpers
{
    public static class ItemHelper
    {
        public static string ItemQualityToClass(this HtmlHelper helper, DataModels.Items.ItemQuality quality)
        {
            switch (quality)
            {
                case DataModels.Items.ItemQuality.Uncommon:
                    return "uncommon";
                default:
                    return "";
            }
        }
    }
}