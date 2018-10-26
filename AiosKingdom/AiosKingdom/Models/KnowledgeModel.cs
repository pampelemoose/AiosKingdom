using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.Models
{
    public class KnowledgeModel
    {
        public Network.Knowledge Knowledge { get; set; }
        public string Name { get; set; }
        public Network.Skills.BookQuality Quality { get; set; }
        public Network.Skills.Page Page { get; set; }
        public bool IsMaxRank { get; set; }
        public int CostToUpdate { get; set; }
    }
}
