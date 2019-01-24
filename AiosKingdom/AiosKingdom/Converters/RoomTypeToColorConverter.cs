using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class RoomTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.AdventureState)
            {
                var adventure = (Network.AdventureState)value;

                if (adventure.IsEliteArea)
                    return Application.Current.Resources["RoomTypeColor_Elite"];

                if (adventure.IsBossFight)
                    return Application.Current.Resources["RoomTypeColor_Boss"];

                return Application.Current.Resources["RoomTypeColor_Normal"];
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
