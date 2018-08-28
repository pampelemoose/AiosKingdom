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

        [Required]
        public string Action { get; set; }

        [Required]
        public string Controller { get; set; }

        [Required]
        public int Order { get; set; }

        public List<Banner> Contents { get; set; }

        [Required]
        public bool Before { get; set; }

        [Required]
        public ContentType Type { get; set; }
    }
}
