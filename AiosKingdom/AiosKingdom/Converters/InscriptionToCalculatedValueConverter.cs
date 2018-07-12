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
            if (value is DataModels.Skills.Inscription && (parameter is string && ((string)parameter == "min" || (string)parameter == "max")))
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
