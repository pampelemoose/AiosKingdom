﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Monsters
{
    public class Phase
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid MonsterId { get; set; }

        [Required]
        public Guid SkillId { get; set; }
    }
}
