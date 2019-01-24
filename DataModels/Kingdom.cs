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

        public string Name { get; set; }
        public int CurrentMaxLevel { get; set; }
        public int LevelGap { get; set; }
        public int MaxLevelCountForGap { get; set; }
        public int CurrentMaxLevelCount { get; set; }

        public List<Town> Towns { get; set; }
    }
}
