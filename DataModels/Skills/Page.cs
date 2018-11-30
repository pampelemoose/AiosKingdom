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

        public string Description { get; set; }
        public int EmberCost { get; set; }
        public int Rank { get; set; }
        public int ManaCost { get; set; }
        public int Cooldown { get; set; }

        public List<Inscription> Inscriptions { get; set; }
    }
}
