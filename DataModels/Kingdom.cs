using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Kingdom
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 100)]
        public int LevelGap { get; set; }

        [Required]
        public int MaxLevelCountForGap { get; set; }

        public List<Config> Towns { get; set; }
    }
}
