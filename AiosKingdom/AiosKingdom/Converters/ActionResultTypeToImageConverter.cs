using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class ActionResultTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.AdventureState.ActionResult.Type)
            {
                var type = (Network.AdventureState.ActionResult.Type)value;

                switch (type)
                {
                    case Network.AdventureState.ActionResult.Type.Damage:
                        return "Assets/Images/ActionResults/damage.png";
                    case Network.AdventureState.ActionResult.Type.Heal:
                        return "Assets/Images/ActionResults/heal.png";
                    case Network.AdventureState.ActionResult.Type.ReceiveMana:
                        return "Assets/Images/ActionResults/receivemana.png";
                    case Network.AdventureState.ActionResult.Type.ConsumedMana:
                        return "Assets/Images/ActionResults/consumedmana.png";
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
