using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Website
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Thread> Threads { get; set; }

        public bool IsActive { get; set; }

        private string _canCreateThreads = "User";
        public string CanCreateThreads
        {
            get { return _canCreateThreads; }
            set
            {
                _canCreateThreads = value;
            }
        }
    }
}
