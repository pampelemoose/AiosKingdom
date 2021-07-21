using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public enum EnemyType
    {
        Normal = 0,
        Elite = 1,
        Boss = 2,
        Rare = 3
    }

    public class Enemy : AVersionized
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MonsterVid { get; set; }

        public EnemyType EnemyType { get; set; }
        public int Level { get; set; }
        public int ShardReward { get; set; }
    }
}
