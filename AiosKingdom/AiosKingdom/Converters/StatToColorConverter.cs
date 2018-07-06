﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class StatToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataModels.Soul.Stats)
            {
                var stat = (DataModels.Soul.Stats)value;

                return Application.Current.Resources["StatColor_" + stat.ToString()];
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
