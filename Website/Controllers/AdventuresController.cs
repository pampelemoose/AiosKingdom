using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class AdventuresController : AKBaseController
    {
        public ActionResult Index(Models.Filters.DungeonFilter filter)
        {
            var dungeons = DataRepositories.AdventureRepository.GetAll();

            filter.Dungeons = filter.FilterList(dungeons);

            return View(filter);
        }

        public ActionResult Details(Guid id)
        {
            var dungeon = DataRepositories.AdventureRepository.GetById(id);

            return View(dungeon);
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpGet]
        public ActionResult Create()
        {
            var dungeon = new Models.DungeonModel();

            dungeon.Rooms = new List<Models.RoomModel>();

            return View(dungeon);
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.DungeonModel dungeonModel)
        {
            if (dungeonModel.Rooms == null)
            {
                dungeonModel.Rooms = new List<Models.RoomModel>();
            }
            foreach (var room in dungeonModel.Rooms)
            {
                if (room.ShopItems == null)
                    room.ShopItems = new List<Models.ShopItemModel>();

                if (room.Enemies == null)
                    room.Enemies = new List<Models.EnemyModel>();
            }

            if (ModelState.IsValid)
            {
                dungeonModel.Rooms?.RemoveAll(r => r.Enemies.Count == 0 && r.ShopItems.Count == 0
                && r.Type != DataModels.Adventures.RoomType.Exit && r.Type != DataModels.Adventures.RoomType.Rest);

                if (dungeonModel.Rooms?.FirstOrDefault(r => r.Type == DataModels.Adventures.RoomType.Exit) == null)
                {
                    Alert(AlertMessage.AlertType.Danger, $"No exit room in the dungeon. You must add an exit as the last room you want the player to leave.", "Exit needed !");
                    return View(dungeonModel);
                }

                if (dungeonModel.Rooms.Count > 0)
                {
                    var dungeonId = Guid.NewGuid();
                    var dungeon = new DataModels.Adventures.Adventure
                    {
                        Id = Guid.NewGuid(),
                        VersionId = dungeonModel.SelectedVersion,
                        DungeonId = dungeonId,
                        Name = dungeonModel.Name,
                        RequiredLevel = dungeonModel.RequiredLevel,
                        MaxLevelAuthorized = dungeonModel.MaxLevelAuthorized,
                        Rooms = new List<DataModels.Adventures.Room>(),
                        ExperienceReward = dungeonModel.ExperienceReward,
                        ShardReward = dungeonModel.ShardReward
                    };

                    foreach (var roomModel in dungeonModel.Rooms)
                    {
                        var roomId = Guid.NewGuid();
                        var room = new DataModels.Adventures.Room
                        {
                            Id = roomId,
                            DungeonId = dungeonId,
                            Type = roomModel.Type,
                            RoomNumber = roomModel.RoomNumber,
                            ShopItems = new List<DataModels.Adventures.ShopItem>(),
                            Ennemies = new List<DataModels.Adventures.Enemy>()
                        };

                        foreach (var shopItemModel in roomModel.ShopItems)
                        {
                            var shopItem = new DataModels.Adventures.ShopItem
                            {
                                Id = Guid.NewGuid(),
                                RoomId = roomId,
                                ItemId = shopItemModel.SelectedItem,
                                Type = shopItemModel.Items.FirstOrDefault(i => i.ItemId.Equals(shopItemModel.SelectedItem)).Type,
                                Quantity = shopItemModel.Quantity,
                                ShardPrice = shopItemModel.ShardPrice
                            };
                            room.ShopItems.Add(shopItem);
                        }

                        foreach (var enemyModel in roomModel.Enemies)
                        {
                            var enemy = new DataModels.Adventures.Enemy
                            {
                                Id = Guid.NewGuid(),
                                RoomId = roomId,
                                EnemyType = enemyModel.EnemyType,
                                MonsterId = enemyModel.MonsterId,
                                Level = enemyModel.Level,
                                ShardReward = enemyModel.ShardReward
                            };
                            room.Ennemies.Add(enemy);
                        }

                        dungeon.Rooms.Add(room);
                    }

                    if (DataRepositories.AdventureRepository.Create(dungeon))
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            return View(dungeonModel);
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpGet]
        public ActionResult AddRoomPartial()
        {
            var room = new Models.RoomModel();
            room.ShopItems = new List<Models.ShopItemModel>();
            room.Enemies = new List<Models.EnemyModel>();

            return PartialView("RoomPartial", room);
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpGet]
        public ActionResult AddShopItemPartial(string id)
        {
            var shopItem = new Models.ShopItemModel();
            shopItem.RoomId = id;

            return PartialView("ShopItemPartial", shopItem);
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpGet]
        public ActionResult AddEnemyPartial(string id)
        {
            var enemy = new Models.EnemyModel();
            enemy.RoomId = id;

            return PartialView("EnemyPartial", enemy);
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var dungeon = DataRepositories.AdventureRepository.GetById(id);

            if (dungeon != null)
            {
                var model = new Models.DungeonModel
                {
                    Id = dungeon.Id,
                    SelectedVersion = dungeon.VersionId,
                    Name = dungeon.Name,
                    RequiredLevel = dungeon.RequiredLevel,
                    MaxLevelAuthorized = dungeon.MaxLevelAuthorized,
                    ExperienceReward = dungeon.ExperienceReward,
                    ShardReward = dungeon.ShardReward,
                    Rooms = new List<Models.RoomModel>()
                };
                
                foreach (var room in dungeon.Rooms)
                {
                    var roomModel = new Models.RoomModel
                    {
                        Id = room.Id,
                        RoomNumber = room.RoomNumber,
                        Type = room.Type,
                        Enemies = new List<Models.EnemyModel>(),
                        ShopItems = new List<Models.ShopItemModel>()
                    };

                    foreach (var shopItem in room.ShopItems)
                    {
                        roomModel.ShopItems.Add(new Models.ShopItemModel
                        {
                            Id = shopItem.Id,
                            SelectedItem = shopItem.ItemId,
                            Quantity = shopItem.Quantity,
                            ShardPrice = shopItem.ShardPrice
                        });
                    }

                    foreach (var ennemy in room.Ennemies)
                    {
                        roomModel.Enemies.Add(new Models.EnemyModel
                        {
                            Id = ennemy.Id,
                            EnemyType = ennemy.EnemyType,
                            MonsterId = ennemy.MonsterId,
                            Level = ennemy.Level,
                            ShardReward = ennemy.ShardReward
                        });
                    }

                    model.Rooms.Add(roomModel);
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.DungeonModel dungeonModel)
        {
            bool newItems = false;
            foreach (var room in dungeonModel.Rooms)
            {
                if (room.NewItems > 0)
                {
                    while (room.NewItems > 0)
                    {
                        if (room.ShopItems == null)
                            room.ShopItems = new List<Models.ShopItemModel>();

                        room.ShopItems.Add(new Models.ShopItemModel());
                        room.NewItems--;
                    }

                    newItems = true;
                }

                if (room.NewEnemies > 0)
                {
                    while (room.NewEnemies > 0)
                    {
                        if (room.Enemies == null)
                            room.Enemies = new List<Models.EnemyModel>();

                        room.Enemies.Add(new Models.EnemyModel());
                        room.NewEnemies--;
                    }

                    newItems = true;
                }
            }
            if (dungeonModel.NewRooms > 0)
            {
                while (dungeonModel.NewRooms > 0)
                {
                    dungeonModel.Rooms.Add(new Models.RoomModel());
                    dungeonModel.NewRooms--;
                }

                newItems = true;
            }

            if (newItems)
                return View(dungeonModel);

            if (dungeonModel.Rooms == null)
            {
                dungeonModel.Rooms = new List<Models.RoomModel>();
            }
            foreach (var room in dungeonModel.Rooms)
            {
                if (room.ShopItems == null)
                    room.ShopItems = new List<Models.ShopItemModel>();

                if (room.Enemies == null)
                    room.Enemies = new List<Models.EnemyModel>();
            }

            if (ModelState.IsValid)
            {
                dungeonModel.Rooms?.RemoveAll(r => r.Enemies.Count == 0 && r.ShopItems.Count == 0
                && r.Type != DataModels.Adventures.RoomType.Exit && r.Type != DataModels.Adventures.RoomType.Rest);

                if (dungeonModel.Rooms?.FirstOrDefault(r => r.Type == DataModels.Adventures.RoomType.Exit) == null)
                {
                    Alert(AlertMessage.AlertType.Danger, $"No exit room in the dungeon. You must add an exit as the last room you want the player to leave.", "Exit needed !");
                    return View(dungeonModel);
                }

                if (dungeonModel.Rooms.Count > 0)
                {
                    var dungeon = new DataModels.Adventures.Adventure
                    {
                        Id = dungeonModel.Id,
                        VersionId = dungeonModel.SelectedVersion,
                        Name = dungeonModel.Name,
                        RequiredLevel = dungeonModel.RequiredLevel,
                        MaxLevelAuthorized = dungeonModel.MaxLevelAuthorized,
                        Rooms = new List<DataModels.Adventures.Room>(),
                        ExperienceReward = dungeonModel.ExperienceReward,
                        ShardReward = dungeonModel.ShardReward
                    };

                    foreach (var roomModel in dungeonModel.Rooms)
                    {
                        var room = new DataModels.Adventures.Room
                        {
                            Id = roomModel.Id,
                            Type = roomModel.Type,
                            RoomNumber = roomModel.RoomNumber,
                            ShopItems = new List<DataModels.Adventures.ShopItem>(),
                            Ennemies = new List<DataModels.Adventures.Enemy>()
                        };

                        foreach (var shopItemModel in roomModel.ShopItems)
                        {
                            var shopItem = new DataModels.Adventures.ShopItem
                            {
                                Id = shopItemModel.Id,
                                ItemId = shopItemModel.SelectedItem,
                                Type = shopItemModel.Items.FirstOrDefault(i => i.ItemId.Equals(shopItemModel.SelectedItem)).Type,
                                Quantity = shopItemModel.Quantity,
                                ShardPrice = shopItemModel.ShardPrice
                            };
                            room.ShopItems.Add(shopItem);
                        }

                        foreach (var enemyModel in roomModel.Enemies)
                        {
                            var enemy = new DataModels.Adventures.Enemy
                            {
                                Id = enemyModel.Id,
                                EnemyType = enemyModel.EnemyType,
                                MonsterId = enemyModel.MonsterId,
                                Level = enemyModel.Level,
                                ShardReward = enemyModel.ShardReward
                            };
                            room.Ennemies.Add(enemy);
                        }

                        dungeon.Rooms.Add(room);
                    }

                    if (DataRepositories.AdventureRepository.Update(dungeon))
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            return View(dungeonModel);
        }
    }
}