using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModels.Adventures
{
    public class Adventure : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int RequiredLevel { get; set; }
        public int MaxLevelAuthorized { get; set; }
        public List<Room> Rooms { get; set; }
        public int ExperienceReward { get; set; }
        public int ShardReward { get; set; }
        public List<Lock> Locks { get; set; }
    }
}
