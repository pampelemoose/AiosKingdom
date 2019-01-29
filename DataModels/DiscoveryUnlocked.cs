using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class DiscoveryUnlocked
    {
        [Key]
        public Guid Id { get; set; }

        public Guid SoulId { get; set; }
        public Guid DiscoveryId { get; set; }

        public DateTime? UnlockedAt { get; set; }
    }
}
