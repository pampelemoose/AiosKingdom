using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class ActionResult
    {
        public enum Type
        {
            PhysicDamage,
            MagicDamage,

            SelfHeal,
            TargetHeal,

            ReceiveMana,
            ConsumedMana,

            EarnExperience,
            EarnShards,
            LevelUp,
            EnemyDeath,
            PlayerDeath
        }

        public Guid FromId { get; set; }
        public Guid ToId { get; set; }

        public Type ResultType { get; set; }
        public Guid Id { get; set; }
        public bool IsConsumable { get; set; }

        public double Amount { get; set; }
    }
}
