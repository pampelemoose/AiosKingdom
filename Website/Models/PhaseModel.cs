using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class PhaseModel
    {
        public class SkillChoice
        {
            public string Name { get; set; }
            public Guid PageId { get; set; }
            public int Rank { get; set; }

            public string PhaseName
            {
                get { return $"{Name} (Rank {Rank})"; }
            }
        }

        public Guid Id { get; set; }

        [Required]
        public Guid SelectedSkill { get; set; }
        [Display(Name = "Skill")]
        public List<SkillChoice> Skills
        {
            get
            {
                var pages = new List<SkillChoice>();

                foreach (var book in DataRepositories.BookRepository.GetAll())
                {
                    foreach (var page in book.Pages)
                    {
                        pages.Add(new SkillChoice
                        {
                            Name = book.Name,
                            PageId = page.Id,
                            Rank = page.Rank
                        });
                    }
                }

                return pages;
            }
        }
    }
}