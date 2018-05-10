using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels
{
    public class Soul
    {
        public enum Stats
        {
            Stamina = 0,
            Energy = 1,
            Strength = 2,
            Agility = 3,
            Intelligence = 4,
            Wisdom = 5
        }

        [Key]
        public Guid Id { get; set; }

        public Guid ServerId { get; set; }
        [ForeignKey("ServerId")]
        public GameServer Server { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [MaxLength(25)]
        public string Name { get; set; }

        public float TimePlayed { get; set; }

        private int _level = 1;
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public int CurrentExperience { get; set; }

        public int Stamina { get; set; }
        public int Energy { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }

        private int _spirits = 25;
        public int Spirits // Stats
        {
            get { return _spirits; }
            set { _spirits = value; }
        }
        public int Embers { get; set; } // Skills
        public int Shards { get; set; } // GameTime Coins
        public int Bits { get; set; } // Basic Coins

        public Guid EquipmentId { get; set; }
        [ForeignKey("EquipmentId")]
        public Equipment Equipment { get; set; }

        public List<InventorySlot> Inventory { get; set; }
        //public List<Datas.Models.Skills.Book> Knowledge { get; set; }
    }
}
