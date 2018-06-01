using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Website
{
    public class Banner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SectionId { get; set; }

        public string Image { get; set; }

        private string _backgroundColor = "#000000";
        [MinLength(7), MaxLength(9)]
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
            }
        }

        [Required]
        public string Content { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
