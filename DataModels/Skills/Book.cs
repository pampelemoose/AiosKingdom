using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public class Book
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public BookQuality Quality { get; set; }

        public List<Page> Pages { get; set; }
    }
}
