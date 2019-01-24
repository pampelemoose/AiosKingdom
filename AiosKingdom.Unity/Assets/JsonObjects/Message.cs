using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects
{
    public struct Message
    {
        public Guid Token { get; set; }
        public int Code { get; set; }
        public bool Success { get; set; }
        public string Json { get; set; }
    }
}
