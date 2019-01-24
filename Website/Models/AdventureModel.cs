using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class AdventureModel
    {
        public Guid Id { get; set; }
        public Guid AdventureId { get; set; }

        [Required]
        [Display(Name = "Version")]
        public Guid SelectedVersion { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "RequiredLevel required")]
        [Display(Name = "RequiredLevel")]
        [Range(1, 10000, ErrorMessage = "RequiredLevel should be higher than 0")]
        public int RequiredLevel { get; set; }

        [Required(ErrorMessage = "MaxLevelAuthorized required")]
        [Display(Name = "MaxLevelAuthorized")]
        [Range(1, 10000, ErrorMessage = "MaxLevelAuthorized should be higher than 0")]
        public int MaxLevelAuthorized { get; set; }

        [Display(Name = "New Rooms")]
        [Range(0, 1000000)]
        public int NewRooms { get; set; }

        [Display(Name = "Rooms")]
        public List<RoomModel> Rooms { get; set; }

        [Required]
        [Range(0, 10000000)]
        public int ExperienceReward { get; set; }

        [Required]
        [Range(0, 10000000)]
        public int ShardReward { get; set; }

        [Display(Name = "Locks")]
        public List<LockModel> Locks { get; set; }

        public AdventureModel()
        {
            Rooms = new List<RoomModel>();
            Locks = new List<LockModel>();
        }
    }

    public class RoomModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Type")]
        public DataModels.Adventures.RoomType Type { get; set; }

        [Required(ErrorMessage = "RoomNumber required")]
        [Display(Name = "RoomNumber")]
        [Range(0, 1000, ErrorMessage = "RoomNumber should be >= 0")]
        public int RoomNumber { get; set; }

        [Display(Name = "New Items")]
        [Range(0, 1000000)]
        public int NewItems { get; set; }

        public List<ShopItemModel> ShopItems { get; set; }

        [Display(Name = "New Enemies")]
        [Range(0, 1000000)]
        public int NewEnemies { get; set; }

        public List<EnemyModel> Enemies { get; set; }

        public RoomModel()
        {
            ShopItems = new List<ShopItemModel>();
            Enemies = new List<EnemyModel>();
        }
    }

    public class LockModel
    {
        [Required]
        [Display(Name = "Locked By")]
        public Guid LockedId { get; set; }
    }

    public class ShopItemModel
    {
        public Guid Id { get; set; }

        public string RoomId { get; set; }

        public Guid SelectedItem { get; set; }
        [Display(Name = "Item")]
        public List<DataModels.Items.Item> Items
        {
            get
            {
                var items = DataRepositories.ItemRepository.GetAll().ToList();

                return items;
            }
        }

        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }

        [Required]
        [Range(1, 100000)]
        public int ShardPrice { get; set; }
    }

    public class EnemyModel
    {
        public Guid Id { get; set; }

        public string RoomId { get; set; }

        [Required]
        public DataModels.Adventures.EnemyType EnemyType { get; set; }

        [Required]
        public Guid MonsterId { get; set; }

        [Required]
        [Range(1, 10000000)]
        public int Level { get; set; }

        [Required]
        [Range(0, 10000000)]
        public int ShardReward { get; set; }
    }
}