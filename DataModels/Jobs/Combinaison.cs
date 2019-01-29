using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Jobs
{
    public class Combinaison : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }

        public double PercentagePerItem { get; set; }

        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}
