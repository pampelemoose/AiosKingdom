using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class NpcDialogue : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid? NextDialogueVid { get; set; }
    }
}
