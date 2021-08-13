using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects.Skills
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

    public enum TalentUnlock
    {
        None,
        Left,
        Next,
        Right
    }

    public enum TalentType
    {
        Cooldown,
        ManaCost,
        BaseValue,
        Ratio,
        Duration,

        StatValue,
    }

    public class Book
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public BookQuality Quality { get; set; }

        public int ExperienceCost { get; set; }
        public int ManaCost { get; set; }
        public int Cooldown { get; set; }

        public List<Inscription> Inscriptions { get; set; }
        public List<Talent> Talents { get; set; }
    }

    public class Talent
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }

        public int Branch { get; set; }
        public int Leaf { get; set; }

        public string InternalUnlocks { get; set; }
        public List<TalentUnlock> Unlocks
        {
            get
            {
                var result = new List<TalentUnlock>();
                if (InternalUnlocks != null)
                {
                    foreach (var str in InternalUnlocks.Split(';'))
                    {
                        if (!String.IsNullOrEmpty(str))
                            result.Add((TalentUnlock)Enum.Parse(typeof(TalentUnlock), str));
                    }
                }
                return result;
            }
        }

        public Guid TargetInscription { get; set; }

        public int TalentPointsRequired { get; set; }

        public TalentType Type { get; set; }
        public double Value { get; set; }
    }

    public class Inscription
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }

        public InscriptionType Type { get; set; }

        public int BaseValue { get; set; }
        public Stats StatType { get; set; }
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
        }

        public float PreferredWeaponDamagesRatio { get; set; }
    }

    public class BuiltSkill
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public BookQuality Quality { get; set; }

        public int ManaCost { get; set; }
        public int Cooldown { get; set; }

        public List<BuiltInscription> Inscriptions { get; set; }
    }

    public class BuiltInscription
    {
        public Guid Id { get; set; }

        public InscriptionType Type { get; set; }

        public int BaseMinValue { get; set; }
        public int BaseMaxValue { get; set; }
        public Stats StatType { get; set; }
        public float Ratio { get; set; }
        public int Duration { get; set; }
    }
}
