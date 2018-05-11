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

        public Items.Bag Bag { get; set; }

        public Items.Armor Head { get; set; }
        public Items.Armor Shoulder { get; set; }
        public Items.Armor Torso { get; set; }
        public Items.Armor Belt { get; set; }
        public Items.Armor Pants { get; set; }
        public Items.Armor Leg { get; set; }
        public Items.Armor Feet { get; set; }
        public Items.Armor Hand { get; set; }
    }
}
