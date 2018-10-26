using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class ActionResultToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.AdventureState.ActionResult)
            {
                var result = (Network.AdventureState.ActionResult)value;

                if (result.IsConsumable)
                {
                    var consumable = DatasManager.Instance.Consumables.FirstOrDefault(c => c.Id.Equals(result.Id));
                    if (consumable != null)
                    {
                        if (!string.IsNullOrEmpty(consumable.Image))
                            return consumable.Image;
                        return "Assets/Images/imgnotfound.png";
                    }
                }
                else
                {
                    var skill = DatasManager.Instance.Books.SelectMany(c => c.Pages).FirstOrDefault(p => p.Id.Equals(result.Id));
                    if (skill != null)
                    {
                        if (!string.IsNullOrEmpty(skill.Image))
                            return skill.Image;
                        return "Assets/Images/imgnotfound.png";
                    }
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
