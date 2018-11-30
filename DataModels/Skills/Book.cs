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

    public class Book
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VersionId { get; set; }
        [Index(IsUnique = true)]
        public Guid BookId { get; set; }

        public string Name { get; set; }
        public BookQuality Quality { get; set; }

        public List<Page> Pages { get; set; }
    }
}
