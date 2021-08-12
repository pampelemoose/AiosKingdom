using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class QuestObjectiveDataLearnBook
    {
        public int NeedToLearnCount { get; set; }

        public List<Guid> Books { get; set; }
    }
}
