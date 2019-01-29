using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Jobs
{
    public enum DiscoveryType
    {
        Adventure,
        Combinaison
    }

    public class Discovery : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public JobType JobType { get; set; }

        public DiscoveryType Type { get; set; }

        public Guid DiscoveredRecipeId { get; set; }
    }
}
