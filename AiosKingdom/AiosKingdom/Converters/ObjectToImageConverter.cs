using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class ObjectToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.Items.Armor)
            {
                var dto = value as Network.Items.Armor;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            if (value is Network.Items.Consumable)
            {
                var dto = value as Network.Items.Consumable;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            if (value is Network.Items.Weapon)
            {
                var dto = value as Network.Items.Weapon;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            if (value is Network.Skills.Page)
            {
                var dto = value as Network.Skills.Page;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            return "Assets/Images/imgnotfound.png";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
