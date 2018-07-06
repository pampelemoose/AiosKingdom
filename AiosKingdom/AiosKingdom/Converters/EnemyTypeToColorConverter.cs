using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class EnemyTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var type = (DataModels.Dungeons.EnemyType)Enum.Parse(typeof(DataModels.Dungeons.EnemyType), (string)value);

                return Application.Current.Resources["EnemyTypeColor_" + type.ToString()];
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
