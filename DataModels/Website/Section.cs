using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Website
{
    public enum ContentType
    {
        Carrousel,
        Row
    }

    public class Section
    {
        [Key]
        public int Id { get; set; }

        public string Action { get; set; }
        public string Controller { get; set; }
        public int Order { get; set; }

        public List<Banner> Contents { get; set; }

        public bool Before { get; set; }
        public ContentType Type { get; set; }
    }
}
