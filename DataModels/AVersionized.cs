using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public abstract class AVersionized
    {
        public Guid VersionId { get; set; }
        [Index(IsUnique = true)]
        public Guid Vid { get; set; }
    }
}
