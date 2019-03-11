using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class RecipeModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Required]
        [Display(Name = "Job")]
        public DataModels.Jobs.JobType JobType { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Price")]
        public int Price { get; set; }

        [Required]
        [Display(Name = "Technique")]
        public DataModels.Jobs.JobTechnique Technique { get; set; }

        public List<DataModels.Jobs.Combinaison> Combinaisons { get; set; }

        [Display(Name = "Common Crafted Item")]
        public Guid? CommonCraftedItemId { get; set; }

        [Display(Name = "Uncommon Crafted Item")]
        public Guid? UncommonCraftedItemId { get; set; }

        [Display(Name = "Rare Crafted Item")]
        public Guid? RareCraftedItemId { get; set; }

        [Display(Name = "Epic Crafted Item")]
        public Guid? EpicCraftedItemId { get; set; }

        [Display(Name = "Legendary Crafted Item")]
        public Guid? LegendaryCraftedItemId { get; set; }

        [Required]
        [Display(Name = "IsShop")]
        public bool IsShop { get; set; }
    }
}