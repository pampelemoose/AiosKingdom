using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class EnemyModel
    {
        public string RoomId { get; set; }

        [Required]
        public DataModels.Dungeons.EnemyType EnemyType { get; set; }

        [Required]
        public Guid MonsterId { get; set; }

        [Required]
        [Range(1, 10000000)]
        public int Level { get; set; }

        [Required]
        [Range(0, 10000000)]
        public int ShardReward { get; set; }
    }
}