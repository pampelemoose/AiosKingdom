using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class InscriptionModel
    {
        public string PageId { get; set; }
        public DataModels.Skills.Inscription Inscription { get; set; }
    }
}