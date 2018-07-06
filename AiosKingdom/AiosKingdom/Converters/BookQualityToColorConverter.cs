using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class BookQualityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataModels.Skills.BookQuality)
            {
                var quality = (DataModels.Skills.BookQuality)value;

                return Application.Current.Resources["BookQualityColor_" + quality.ToString()];
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
