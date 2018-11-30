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

        public int SectionId { get; set; }

        public string Image { get; set; }

        public ImageSide ImageSide { get; set; }

        private string _backgroundColor = "#000000";
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
            }
        }

        private string _textColor = "#FFFFFF";
        public string TextColor
        {
            get { return _textColor; }
            set
            {
                _textColor = value;
            }
        }

        public string Content { get; set; }
        public int Order { get; set; }
        public bool Active { get; set; }
    }
}
