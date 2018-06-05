using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Monsters
{
    public enum MonsterType
    {
        Human = 0
    }

    public class Monster
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VersionId { get; set; }

        [Required]
        public Guid MonsterId { get; set; }

        [Required]
        public string InternalTypes { get; set; }
        public List<MonsterType> Types
        {
            get
            {
                var result = new List<MonsterType>();
                foreach (var str in InternalTypes.Split(';'))
                {
                    result.Add((MonsterType)Enum.Parse(typeof(MonsterType), str));
                }
                return result;
            }
            set
            {
                InternalTypes = String.Join(";", value);
            }
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Story { get; set; }

        private string _image = "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";
        [Display(Name = "Image")]
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        [Required]
        [Range(1, 10000)]
        public double HealthPerLevel { get; set; }

        [Required]
        public int BaseExperience { get; set; }

        [Required]
        [Range(0.001, 100)]
        public double ExperiencePerLevelRatio { get; set; }

        [Required]
        public List<Loot> Loots { get; set; }

        [Required]
        public List<Phase> Phases { get; set; }

        public List<Dungeons.Room> Rooms { get; set; }
    }
}
