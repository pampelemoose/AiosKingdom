using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class ItemQualityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataModels.Items.ItemQuality)
            {
                var quality = (DataModels.Items.ItemQuality)value;

                switch (quality)
                {
                    case DataModels.Items.ItemQuality.Common:
                        return Application.Current.Resources["ItemQualityColor_Common"];
                    case DataModels.Items.ItemQuality.Uncommon:
                        return Application.Current.Resources["ItemQualityColor_Uncommon"];
                    case DataModels.Items.ItemQuality.Rare:
                        return Application.Current.Resources["ItemQualityColor_Rare"];
                    case DataModels.Items.ItemQuality.Epic:
                        return Application.Current.Resources["ItemQualityColor_Epic"];
                    case DataModels.Items.ItemQuality.Legendary:
                        return Application.Current.Resources["ItemQualityColor_Legendary"];
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
