using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class BookstoreItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid BookstoreId { get; set; }
        public Guid BookVid { get; set; }
    }
}
