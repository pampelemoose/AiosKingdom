using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Skills
{
    public enum InscriptionType
    {
        PhysicDamages = 0,
        MagicDamages = 1,

        SelfHeal = 2,
        TargetHeal = 3,

        StatBuff = 4,
        StatDebuff = 5,

        Stunt = 6,
        Charm = 7,
        Confuse = 8,

        Unstunt = 10,
        Uncharm = 11,
        Unconfuse = 12,

        // ADVANCED IDEAS
        Burn = 15,
        Freeze = 16,

        Unburn = 17,
        Unfreeze = 18,

        Multistrike = 20
    }

    public class Inscription
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BookId { get; set; }

        public InscriptionType Type { get; set; }
        public int BaseValue { get; set; }
        public Soul.Stats StatType { get; set; }
        public float Ratio { get; set; }
        public int Duration { get; set; }

        public bool IncludeWeaponDamages { get; set; }

        public string InternalWeaponTypes { get; set; }
        public List<Items.ItemType> WeaponTypes
        {
            get
            {
                var result = new List<Items.ItemType>();
                if (InternalWeaponTypes != null)
                {
                    foreach (var str in InternalWeaponTypes.Split(';'))
                    {
                        if (!String.IsNullOrEmpty(str))
                            result.Add((Items.ItemType)Enum.Parse(typeof(Items.ItemType), str));
                    }
                }
                return result;
            }
            set
            {
                if (value != null)
                    InternalWeaponTypes = String.Join(";", value);
            }
        }

        public float WeaponDamagesRatio { get; set; }

        public string InternalPreferredWeaponTypes { get; set; }
        public List<Items.ItemType> PreferredWeaponTypes
        {
            get
            {
                var result = new List<Items.ItemType>();
                if (InternalPreferredWeaponTypes != null)
                {
                    foreach (var str in InternalPreferredWeaponTypes.Split(';'))
                    {
                        if (!String.IsNullOrEmpty(str))
                            result.Add((Items.ItemType)Enum.Parse(typeof(Items.ItemType), str));
                    }
                }
                return result;
            }
            set
            {
                if (value != null)
                    InternalPreferredWeaponTypes = String.Join(";", value);
            }
        }

        public float PreferredWeaponDamagesRatio { get; set; }

    }
}
