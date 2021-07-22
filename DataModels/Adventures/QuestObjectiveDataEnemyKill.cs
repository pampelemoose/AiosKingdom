using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class QuestObjectiveDataEnemyKill
    {

        public Guid EnemyVid { get; set; }
        public int KillCount { get; set; }
    }
}
