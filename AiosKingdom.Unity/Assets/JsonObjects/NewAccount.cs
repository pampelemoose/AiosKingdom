using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects
{
    public class NewAccount
    {
        public Guid Identifier { get; set; }
        public Guid SafeKey { get; set; }
        public Guid PublicKey { get; set; }
        public int SoulSlots { get; set; }
    }
}
