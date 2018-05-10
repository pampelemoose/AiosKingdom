using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels
{
    public class Equipment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid Bag { get; set; }
        public Guid Head { get; set; }
        public Guid Shoulder { get; set; }
        public Guid Torso { get; set; }
        public Guid Belt { get; set; }
        public Guid Pants { get; set; }
        public Guid Leg { get; set; }
        public Guid Feet { get; set; }
        public Guid Hand { get; set; }
    }
}
