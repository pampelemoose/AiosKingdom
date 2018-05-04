using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels
{
    public class UserServerSlot
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ServerId { get; set; }
        public Guid SoulId { get; set; }
    }
}
