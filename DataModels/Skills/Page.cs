using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Skills
{
    public class Page
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BookId { get; set; }
        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required, MaxLength(50)]
        public string Description { get; set; }

        private string _image = "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        [Required]
        public int StatCost { get; set; }

        [Required]
        public int Rank { get; set; }

        [Required]
        public int ManaCost { get; set; }

        public List<Inscription> Inscriptions { get; set; }
    }
}
