﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class ListToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Network.Items.ItemEffect>)
            {
                var list = value as List<Network.Items.ItemEffect>;
                var height = int.Parse((string)parameter);
                return list.Count * height;
            }

            if (value is List<Network.Items.ItemStat>)
            {
                var list = value as List<Network.Items.ItemStat>;
                var height = int.Parse((string)parameter);
                return list.Count * height;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
