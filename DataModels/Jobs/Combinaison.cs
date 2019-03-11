using System;
using System.ComponentModel.DataAnnotations;

namespace DataModels.Jobs
{
    public class Combinaison
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RecipeId { get; set; }

        public Guid? CommonItemId { get; set; }
        public Guid? UncommonItemId { get; set; }
        public Guid? RareItemId { get; set; }
        public Guid? EpicItemId { get; set; }
        public Guid? LegendaryItemId { get; set; }

        public double PercentagePerItem { get; set; }

        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }

        public Guid? ItemId(Items.ItemQuality quality)
        {
            switch (quality)
            {
                case Items.ItemQuality.Common:
                    return CommonItemId;
                case Items.ItemQuality.Uncommon:
                    return UncommonItemId;
                case Items.ItemQuality.Rare:
                    return RareItemId;
                case Items.ItemQuality.Epic:
                    return EpicItemId;
                case Items.ItemQuality.Legendary:
                    return LegendaryItemId;
            }

            return null;
        }
    }
}
