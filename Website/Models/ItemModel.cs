using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class ItemModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quality required")]
        [Display(Name = "Quality")]
        public DataModels.Items.ItemQuality Quality { get; set; }

        [Required(ErrorMessage = "Type required")]
        [Display(Name = "Type")]
        public DataModels.Items.ItemType Type { get; set; }

        [Display(Name = "Slot")]
        public DataModels.Items.ItemSlot? Slot { get; set; }

        [Required(ErrorMessage = "ItemLevel required")]
        [Display(Name = "Item Level")]
        [Range(1, 400, ErrorMessage = "ItemLevel should be higher than 0")]
        public int ItemLevel { get; set; }

        [Required(ErrorMessage = "UseLevelRequired required")]
        [Display(Name = "Level Required")]
        [Range(1, 400)]
        public int UseLevelRequired { get; set; }

        [Required(ErrorMessage = "Space required")]
        [Display(Name = "Space")]
        [Range(1, 400)]
        public int Space { get; set; }

        [Required(ErrorMessage = "SellingPrice required")]
        [Display(Name = "SellingPrice")]
        [Range(1, 10000000)]
        public int SellingPrice { get; set; }

        [Display(Name = "Armor Value")]
        [Range(1, 400)]
        public int? ArmorValue { get; set; }

        [Display(Name = "Slot Count")]
        [Range(1, 400)]
        public int? SlotCount { get; set; }

        [Display(Name = "MinDamages")]
        [Range(1, 400)]
        public int? MinDamages { get; set; }

        [Display(Name = "MaxDamages")]
        [Range(1, 400)]
        public int? MaxDamages { get; set; }

        [Display(Name = "Stats")]
        public List<DataModels.Items.ItemStat> Stats { get; set; }

        [Display(Name = "Effects")]
        public List<DataModels.Items.ItemEffect> Effects { get; set; }

        public ItemModel()
        {
            SetupLists();
        }

        public void SetupLists()
        {
            if (Stats == null)
            {
                Stats = new List<DataModels.Items.ItemStat>();
            }

            if (Effects == null)
            {
                Effects = new List<DataModels.Items.ItemEffect>();
            }

            foreach (DataModels.Soul.Stats en in Enum.GetValues(typeof(DataModels.Soul.Stats)))
            {
                if (!Stats.Any(s => s.Type == en))
                {
                    Stats.Add(new DataModels.Items.ItemStat
                    {
                        Type = en
                    });
                }
            }
        }
    }
}