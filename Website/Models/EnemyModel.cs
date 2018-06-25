﻿using System;
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
        public Guid MonsterId { get; set; }

        public List<DataModels.Monsters.Monster> AvailableMonsters
        {
            get
            {
                return DataRepositories.MonsterRepository.GetAll();
            }
        }

        [Required]
        [Range(1, 10000)]
        public int Level { get; set; }
    }
}