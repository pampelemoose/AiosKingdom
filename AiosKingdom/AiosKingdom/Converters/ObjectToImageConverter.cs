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
            if (value is DataModels.Items.Armor)
            {
                var dto = value as DataModels.Items.Armor;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            if (value is DataModels.Items.Consumable)
            {
                var dto = value as DataModels.Items.Consumable;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            if (value is DataModels.Items.Weapon)
            {
                var dto = value as DataModels.Items.Weapon;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            if (value is DataModels.Skills.Page)
            {
                var dto = value as DataModels.Skills.Page;

                if (!string.IsNullOrEmpty(dto.Image))
                    return dto.Image;
            }

            return "http://startyoungfinancial.com/wp-content/uploads/2015/05/default-image.png";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
