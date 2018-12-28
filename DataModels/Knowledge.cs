using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Knowledge
    {
        [Key]
        public Guid Id { get; set; }

        public Guid SoulId { get; set; }
        public Guid BookVid { get; set; }

        public int TalentPoints { get; set; }

        public List<TalentUnlocked> Talents { get; set; }
    }
}
