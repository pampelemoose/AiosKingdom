using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class AdventureProgress
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid SoulId { get; set; }

        [Required]
        public Guid DungeonId { get; set; }

        [Required]
        public int CurrentRoom { get; set; }
    } 
}
