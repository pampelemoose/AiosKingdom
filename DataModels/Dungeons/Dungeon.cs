using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataModels.Dungeons
{
    public class Dungeon
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VersionId { get; set; }

        [Required]
        public Guid DungeonId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 10000)]
        public int RequiredLevel { get; set; }

        [Required]
        [Range(1, 10000)]
        public int MaxLevelAuthorized { get; set; }

        [Required]
        public List<Room> Rooms { get; set; }
    }
}
