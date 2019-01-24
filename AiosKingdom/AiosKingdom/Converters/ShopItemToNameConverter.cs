using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class ShopItemToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.AdventureState.ShopState)
            {
                var loot = (Network.AdventureState.ShopState)value;

                Network.Items.ItemType type = (Network.Items.ItemType)Enum.Parse(typeof(Network.Items.ItemType), loot.Type);

                switch (type)
                {
                    case Network.Items.ItemType.Armor:
                        return DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(loot.ItemId))?.Name;
                    case Network.Items.ItemType.Bag:
                        return DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(loot.ItemId))?.Name;
                    case Network.Items.ItemType.Consumable:
                        return DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(loot.ItemId))?.Name;
                    case Network.Items.ItemType.Junk:
                        break;
                    default:
                        return DatasManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(loot.ItemId))?.Name;
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
