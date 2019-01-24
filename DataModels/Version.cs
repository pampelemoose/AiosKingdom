using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Version
    {
        [Key]
        public Guid Id { get; set; }

        public int Low { get; set; }
        public int Mid { get; set; }
        public int High { get; set; }

        public override string ToString()
        {
            return $"{High}.{Mid}.{Low}";
        }
    }
}
