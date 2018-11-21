using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class ItemFilter
    {
        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Quality")]
        public DataModels.Items.ItemQuality? Quality { get; set; }
        [Display(Name = "ItemLevel")]
        public int ItemLevel { get; set; }
        [Display(Name = "LevelRequired")]
        public int UseLevelRequired { get; set; }

        [Display(Name = "Type")]
        public DataModels.Items.ItemType? Type { get; set; }

        [Display(Name = "Slot")]
        public DataModels.Items.ItemSlot? Slot { get; set; }

        [Display(Name = "SlotCount")]
        public int? SlotCount { get; set; }

        [Display(Name = "Stat")]
        public DataModels.Soul.Stats? Stat { get; set; }

        [Display(Name = "Effect")]
        public DataModels.Items.EffectType? Effect { get; set; }

        public List<DataModels.Items.Item> Items { get; set; }

        public List<DataModels.Items.Item> FilterList(List<DataModels.Items.Item> list)
        {
            if (!Guid.Empty.Equals(SelectedVersion))
            {
                list = list.Where(a => a.VersionId.Equals(SelectedVersion)).ToList();
            }

            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }

            if (Quality != null)
            {
                list = list.Where(a => a.Quality.Equals(Quality)).ToList();
            }

            if (ItemLevel > 0)
            {
                list = list.Where(a => a.ItemLevel.Equals(ItemLevel)).ToList();
            }

            if (UseLevelRequired > 0)
            {
                list = list.Where(a => a.UseLevelRequired.Equals(UseLevelRequired)).ToList();
            }

            if (Type != null)
            {
                list = list.Where(a => a.Type.Equals(Type)).ToList();
            }

            if (Slot != null)
            {
                list = list.Where(a => a.Slot.Equals(Slot)).ToList();
            }

            if (SlotCount!= null)
            {
                list = list.Where(a => a.SlotCount.Equals(SlotCount)).ToList();
            }

            if (Stat != null)
            {
                list = list.Where(a => a.Stats.Any(e => e.Type.Equals(Stat))).ToList();
            }

            if (Effect != null)
            {
                list = list.Where(a => a.Effects.Any(e => e.Type.Equals(Effect))).ToList();
            }

            return list;
        }
    }
}