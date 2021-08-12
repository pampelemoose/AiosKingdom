using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Skills
{
    public enum BookQuality
    {
        TierFive = 0,
        TierFour = 1,
        TierThree = 2,
        TierTwo = 3,
        TierOne = 4
    }

    public enum BookAction
    {
        Slash,
        Thrust,
        Hammer,
        Cast,
        Parry,
        Evade,
        Shoot,
        Hit,
        Concentrate
    }

    public class Book : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public BookQuality Quality { get; set; }

        public BookAction Action { get; set; }
        public int Repetition { get; set; }

        public bool RequireWeapon { get; set; }
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

        public int EmberCost { get; set; }
        public int ManaCost { get; set; }
        public int Cooldown { get; set; }

        public List<Inscription> Inscriptions { get; set; }
        public List<Talent> Talents { get; set; }
    }
}
