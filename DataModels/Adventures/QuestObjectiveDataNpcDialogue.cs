using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class QuestObjectiveDataNpcDialogue
    {
        public Guid NpcVid { get; set; }
        public Guid NpcDialogueVid { get; set; }
    }
}
