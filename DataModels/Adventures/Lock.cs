using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class Lock
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AdventureId { get; set; }
        public Guid LockedId { get; set; }
    }
}
