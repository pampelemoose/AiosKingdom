using System;
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

                switch (stat)
                {
                    case DataModels.Soul.Stats.Stamina:
                        return Application.Current.Resources["StatColor_Stamina"];
                    case DataModels.Soul.Stats.Energy:
                        return Application.Current.Resources["StatColor_Energy"];
                    case DataModels.Soul.Stats.Strength:
                        return Application.Current.Resources["StatColor_Strength"];
                    case DataModels.Soul.Stats.Agility:
                        return Application.Current.Resources["StatColor_Agility"];
                    case DataModels.Soul.Stats.Intelligence:
                        return Application.Current.Resources["StatColor_Intelligence"];
                    case DataModels.Soul.Stats.Wisdom:
                        return Application.Current.Resources["StatColor_Wisdom"];
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
