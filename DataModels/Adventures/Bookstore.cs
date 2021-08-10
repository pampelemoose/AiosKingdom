using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class Bookstore : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<BookstoreItem> Books { get; set; }
    }
}
