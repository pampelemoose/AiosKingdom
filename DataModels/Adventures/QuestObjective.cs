using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class QuestObjective : AVersionized
    {
        public enum ObjectiveType
        {
            EnemyKill,
            NpcDialogue,
            ExploreArea
        }

        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public ObjectiveType Type { get; set; }

        public string ObjectiveDataJson { get; set; }
    }
}
