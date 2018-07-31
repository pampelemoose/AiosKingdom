using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class AppUser
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid Identifier { get; set; }

        [Required]
        public Guid SafeKey { get; set; }

        [Required]
        public Guid PublicKey { get; set; }

        private int _soulSlots = 1;
        public int SoulSlots
        {
            get { return _soulSlots; }
            set { _soulSlots = value; }
        }

        public List<Soul> Souls { get; set; }
    }
}
