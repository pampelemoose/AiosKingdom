using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class InscriptionToCalculatedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataModels.Skills.Inscription)
            {
                var insc = (DataModels.Skills.Inscription)value;
                int statVal = 0;

                switch (insc.StatType)
                {
                    case DataModels.Soul.Stats.Stamina:
                        statVal = DatasManager.Instance.Datas.TotalStamina;
                        break;
                    case DataModels.Soul.Stats.Energy:
                        statVal = DatasManager.Instance.Datas.TotalEnergy;
                        break;
                    case DataModels.Soul.Stats.Strength:
                        statVal = DatasManager.Instance.Datas.TotalStrength;
                        break;
                    case DataModels.Soul.Stats.Agility:
                        statVal = DatasManager.Instance.Datas.TotalAgility;
                        break;
                    case DataModels.Soul.Stats.Intelligence:
                        statVal = DatasManager.Instance.Datas.TotalIntelligence;
                        break;
                    case DataModels.Soul.Stats.Wisdom:
                        statVal = DatasManager.Instance.Datas.TotalWisdom;
                        break;
                }

                return insc.BaseValue + (statVal * insc.Ratio);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
