using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Skills
{
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

    public class Talent
    {
        [Key]
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
            set
            {
                if (value != null)
                    InternalUnlocks = String.Join(";", value);
            }
        }

        public Guid TargetInscription { get; set; }

        public int TalentPointsRequired { get; set; }

        public TalentType Type { get; set; }
        public double Value { get; set; }
    }
}
