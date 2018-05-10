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
            /*if (value is Guid)
            {
                var dto = value as DataDtos.ArmorDto;

                if (!string.IsNullOrEmpty(dto.Armor.Image))
                    return dto.Armor.Image;
            }

            if (value is DataDtos.ConsumableDto)
            {
                var dto = value as DataDtos.ConsumableDto;

                if (!string.IsNullOrEmpty(dto.Consumable.Image))
                    return dto.Consumable.Image;
            }*/

            return "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
