using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Skills
{
    public class Page
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BookId { get; set; }

        [Required, MaxLength(400)]
        public string Description { get; set; }

        [Required]
        public int EmberCost { get; set; }

        [Required]
        public int Rank { get; set; }

        [Required]
        public int ManaCost { get; set; }

        [Required]
        public int Cooldown { get; set; }

        public List<Inscription> Inscriptions { get; set; }
    }
}
