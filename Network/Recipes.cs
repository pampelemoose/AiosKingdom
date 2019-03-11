using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public enum JobTechnique
    {
        Shatter,
        Sand,
        Shape,

        Chop,
        Cut,

        Carve,
        Write,

        Fold,
        Meld,
        Bend,

        Melt,
        Cool,

        Boil,
        Blow,
        Dip,
        Dry,

        Assemble,
        Mix
    }

    public class Recipe
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public JobType JobType { get; set; }

        public int Price { get; set; }

        public JobTechnique Technique { get; set; }

        public List<Combinaison> Combinaisons { get; set; }

        public Guid? CommonCraftedItemId { get; set; }
        public Guid? UncommonCraftedItemId { get; set; }
        public Guid? RareCraftedItemId { get; set; }
        public Guid? EpicCraftedItemId { get; set; }
        public Guid? LegendaryCraftedItemId { get; set; }

        public Guid? CraftedItemId(Items.ItemQuality quality)
        {
            switch (quality)
            {
                case Items.ItemQuality.Common:
                    return CommonCraftedItemId;
                case Items.ItemQuality.Uncommon:
                    return UncommonCraftedItemId;
                case Items.ItemQuality.Rare:
                    return RareCraftedItemId;
                case Items.ItemQuality.Epic:
                    return EpicCraftedItemId;
                case Items.ItemQuality.Legendary:
                    return LegendaryCraftedItemId;
            }

            return null;
        }
    }

    public class Combinaison
    {
        public Guid Id { get; set; }

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
