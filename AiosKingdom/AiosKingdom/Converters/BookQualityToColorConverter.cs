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

                switch (quality)
                {
                    case DataModels.Skills.BookQuality.TierFive:
                        return Application.Current.Resources["BookQualityColor_TierFive"];
                    case DataModels.Skills.BookQuality.TierFour:
                        return Application.Current.Resources["BookQualityColor_TierFour"];
                    case DataModels.Skills.BookQuality.TierThree:
                        return Application.Current.Resources["BookQualityColor_TierThree"];
                    case DataModels.Skills.BookQuality.TierTwo:
                        return Application.Current.Resources["BookQualityColor_TierTwo"];
                    case DataModels.Skills.BookQuality.TierOne:
                        return Application.Current.Resources["BookQualityColor_TierOne"];
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
