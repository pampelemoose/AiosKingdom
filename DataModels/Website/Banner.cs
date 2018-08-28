using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Website
{
    public enum ImageSide
    {
        Left,
        Right
    }

    public class Banner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SectionId { get; set; }

        public string Image { get; set; }

        public ImageSide ImageSide { get; set; }

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

        private string _textColor = "#FFFFFF";
        [MinLength(7), MaxLength(9)]
        public string TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
            }
        }

        [Required]
        public string Content { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
