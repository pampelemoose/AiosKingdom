using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels.Items
{
    public enum ArmorPart
    {
        Head = 0,
        Shoulder = 1,
        Torso = 2,
        Belt = 3,
        Pants = 4,
        Leg = 5,
        Feet = 6,
        Hand = 7
    }

    public class Armor : AItem
    {
        [Required(ErrorMessage = "Part required")]
        [Display(Name = "Part")]
        public ArmorPart Part { get; set; }

        [Required(ErrorMessage = "ArmorValue required")]
        [Display(Name = "Armor Value")]
        [Range(0, 400)]
        public int ArmorValue { get; set; }

        [Display(Name = "Stats")]
        public List<ItemStat> Stats { get; set; }

        public Armor()
            : base(ItemType.Armor)
        {
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.Length < 4)
                yield return new ValidationResult("Must enter a name (min 4 char)", new[] { "Name" });

            if (Description.Length < 4)
                yield return new ValidationResult("Must enter a description (min 4 char)", new[] { "Description" });

            if (ItemLevel < 1)
                yield return new ValidationResult("Item Level must be superior than 0", new[] { "ItemLevel" });
        }
    }
}
