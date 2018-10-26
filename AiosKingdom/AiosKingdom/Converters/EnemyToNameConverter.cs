using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class EnemyToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.AdventureState.EnemyState)
            {
                var enemy = (Network.AdventureState.EnemyState)value;

                return DatasManager.Instance.Monsters.FirstOrDefault(e => e.Id.Equals(enemy.MonsterId))?.Name;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
