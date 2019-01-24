using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AiosKingdom.Converters
{
    public class InscriptionToCalculatedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Network.Skills.Inscription && (parameter is string && ((string)parameter == "min" || (string)parameter == "max")))
            {
                var insc = (Network.Skills.Inscription)value;
                int statVal = 0;

                switch (insc.StatType)
                {
                    case Network.Stats.Stamina:
                        statVal = DatasManager.Instance.Datas.TotalStamina;
                        break;
                    case Network.Stats.Energy:
                        statVal = DatasManager.Instance.Datas.TotalEnergy;
                        break;
                    case Network.Stats.Strength:
                        statVal = DatasManager.Instance.Datas.TotalStrength;
                        break;
                    case Network.Stats.Agility:
                        statVal = DatasManager.Instance.Datas.TotalAgility;
                        break;
                    case Network.Stats.Intelligence:
                        statVal = DatasManager.Instance.Datas.TotalIntelligence;
                        break;
                    case Network.Stats.Wisdom:
                        statVal = DatasManager.Instance.Datas.TotalWisdom;
                        break;
                }

                double wpDmg = 0;
                if (insc.IncludeWeaponDamages)
                {
                    if (insc.WeaponTypes.Where(w => DatasManager.Instance.Datas.WeaponTypes.Contains(w.ToString())).Count() > 0)
                    {
                        wpDmg += (((string)parameter == "min") ? DatasManager.Instance.Datas.MinDamages : DatasManager.Instance.Datas.MaxDamages) * insc.WeaponDamagesRatio;
                    }
                    if (insc.PreferredWeaponTypes.Where(w => DatasManager.Instance.Datas.WeaponTypes.Contains(w.ToString())).Count() > 0)
                    {
                        wpDmg += (((string)parameter == "min") ? DatasManager.Instance.Datas.MinDamages : DatasManager.Instance.Datas.MaxDamages) * insc.PreferredWeaponDamagesRatio;
                    }
                }

                return insc.BaseValue + (statVal * insc.Ratio) + wpDmg;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
