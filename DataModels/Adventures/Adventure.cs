﻿using System;
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

        public bool Repeatable { get; set; }

        public List<Quest> Quests { get; set; }

        public Guid MapIdentifier { get; set; }
        public int SpawnCoordinateX { get; set; }
        public int SpawnCoordinateY { get; set; }

        public int ExperienceReward { get; set; }
        public int ShardReward { get; set; }
        public List<Lock> Locks { get; set; }
    }
}
