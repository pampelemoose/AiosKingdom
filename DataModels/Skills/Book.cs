using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Skills
{
    public enum BookQuality
    {
        TierFive = 0,
        TierFour = 1,
        TierThree = 2,
        TierTwo = 3,
        TierOne = 4
    }

    public class Book : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public BookQuality Quality { get; set; }

        public int EmberCost { get; set; }
        public int ManaCost { get; set; }
        public int Cooldown { get; set; }

        public List<Inscription> Inscriptions { get; set; }
        public List<Talent> Talents { get; set; }
    }
}
