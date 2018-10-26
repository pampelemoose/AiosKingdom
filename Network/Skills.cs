using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Skills
{
    public enum BookQuality
    {
        TierFive = 0,
        TierFour = 1,
        TierThree = 2,
        TierTwo = 3,
        TierOne = 4
    }

    public enum InscriptionType
    {
        Damages = 0,
        Heal = 1,

        StatBuff = 2,
        StatDebuff = 3,

        Stunned = 4,
        Charmed = 5,
        Confused = 6,

        // ADVANCED IDEAS
        Burn = 15,
        Frozen = 16
    }

    public enum ElementType
    {
        Neutral = 0,

        Fire = 1,
        Water = 2,
        Wind = 3,
        Earth = 4,
        Lightning = 5,

        // ADVANCED IDEAS
        Light = 6,
        Shadow = 7,

        Ice = 8,
        Magma = 9,

        Life = 10,
        Death = 11
    }

    public class Book
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public BookQuality Quality { get; set; }

        public List<Page> Pages { get; set; }
    }

    public class Page
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
        public string Image { get; set; }

        public int EmberCost { get; set; }
        public int Rank { get; set; }
        public int ManaCost { get; set; }
        public int Cooldown { get; set; }

        public List<Inscription> Inscriptions { get; set; }
    }

    public class Inscription
    {
        public Guid Id { get; set; }
        public Guid PageId { get; set; }

        public InscriptionType Type { get; set; }

        public int BaseValue { get; set; }
        public Stats StatType { get; set; }
        public float Ratio { get; set; }
        public int Duration { get; set; }

        public bool IncludeWeaponDamages { get; set; }

        public string InternalWeaponTypes { get; set; }
        public List<Items.WeaponType> WeaponTypes
        {
            get
            {
                var result = new List<Items.WeaponType>();
                if (InternalWeaponTypes != null)
                {
                    foreach (var str in InternalWeaponTypes.Split(';'))
                    {
                        if (!String.IsNullOrEmpty(str))
                            result.Add((Items.WeaponType)Enum.Parse(typeof(Items.WeaponType), str));
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
        public List<Items.WeaponType> PreferredWeaponTypes
        {
            get
            {
                var result = new List<Items.WeaponType>();
                if (InternalPreferredWeaponTypes != null)
                {
                    foreach (var str in InternalPreferredWeaponTypes.Split(';'))
                    {
                        if (!String.IsNullOrEmpty(str))
                            result.Add((Items.WeaponType)Enum.Parse(typeof(Items.WeaponType), str));
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
