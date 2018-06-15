using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.Models.Dungeon
{
    public class SkillSelectionItemModel
    {
        public string BookName { get; set; }
        public DataModels.Skills.Page Skill { get; set; }
        public bool CanSelect { get; set; }
    }
}
