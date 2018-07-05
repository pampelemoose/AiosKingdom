using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class LootItemToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.LootItem)
            {
                var loot = (Network.LootItem)value;

                DataModels.Items.ItemType type = (DataModels.Items.ItemType)Enum.Parse(typeof(DataModels.Items.ItemType), loot.Type);

                switch (type)
                {
                    case DataModels.Items.ItemType.Armor:
                        return DatasManager.Instance.Armors.FirstOrDefault(a => a.ItemId.Equals(loot.ItemId))?.Name;
                    case DataModels.Items.ItemType.Bag:
                        return DatasManager.Instance.Bags.FirstOrDefault(a => a.ItemId.Equals(loot.ItemId))?.Name;
                    case DataModels.Items.ItemType.Consumable:
                        return DatasManager.Instance.Consumables.FirstOrDefault(a => a.ItemId.Equals(loot.ItemId))?.Name;
                }
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
