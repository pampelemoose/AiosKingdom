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
        Boss = 2
    }

    public class Enemy
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public Guid MonsterId { get; set; }

        [Required]
        public EnemyType EnemyType { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public int ShardReward { get; set; }
    }
}
