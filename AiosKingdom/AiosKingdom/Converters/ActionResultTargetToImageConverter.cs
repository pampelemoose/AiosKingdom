using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class ActionResultTargetToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid)
            {
                var enemyId = (Guid)value;
                var enemy = DatasManager.Instance.Monsters.FirstOrDefault(m => m.MonsterId.Equals(enemyId));
                if (enemy != null)
                {
                    if (!string.IsNullOrEmpty(enemy.Image))
                        return enemy.Image;
                    return "Assets/Images/imgnotfound.png";
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
