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
            return View(new Models.AdventureModel());
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.AdventureModel dungeonModel)
        {
            if (dungeonModel.Rooms == null)
            {
                dungeonModel.Rooms = new List<Models.RoomModel>();
            }
            if (dungeonModel.Locks == null)
            {
                dungeonModel.Locks = new List<Models.LockModel>();
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
                    var adventureId = Guid.NewGuid();
                    var dungeon = new DataModels.Adventures.Adventure
                    {
                        Id = Guid.NewGuid(),
                        VersionId = dungeonModel.SelectedVersion,
                        AdventureId = adventureId,
                        Name = dungeonModel.Name,
                        RequiredLevel = dungeonModel.RequiredLevel,
                        MaxLevelAuthorized = dungeonModel.MaxLevelAuthorized,
                        Rooms = new List<DataModels.Adventures.Room>(),
                        ExperienceReward = dungeonModel.ExperienceReward,
                        ShardReward = dungeonModel.ShardReward,
                        Locks = new List<DataModels.Adventures.Lock>()
                    };

                    foreach (var roomModel in dungeonModel.Rooms)
                    {
                        var roomId = Guid.NewGuid();
                        var room = new DataModels.Adventures.Room
                        {
                            Id = roomId,
                            AdventureId = adventureId,
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
                                Price = shopItemModel.ShardPrice
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

                    foreach (var lockModel in dungeonModel.Locks)
                    {
                        var locked = new DataModels.Adventures.Lock
                        {
                            Id = Guid.NewGuid(),
                            AdventureId = adventureId,
                            LockedId = lockModel.LockedId
                        };
                        dungeon.Locks.Add(locked);
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
        public ActionResult AddLockPartial()
        {
            var locked = new Models.LockModel();

            return PartialView("LockPartial", locked);
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
            var adventure = DataRepositories.AdventureRepository.GetById(id);

            if (adventure != null)
            {
                var model = new Models.AdventureModel
                {
                    Id = adventure.Id,
                    AdventureId = adventure.AdventureId,
                    SelectedVersion = adventure.VersionId,
                    Name = adventure.Name,
                    RequiredLevel = adventure.RequiredLevel,
                    MaxLevelAuthorized = adventure.MaxLevelAuthorized,
                    ExperienceReward = adventure.ExperienceReward,
                    ShardReward = adventure.ShardReward,
                    Rooms = new List<Models.RoomModel>(),
                    Locks = new List<Models.LockModel>()
                };
                
                foreach (var room in adventure.Rooms)
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
                            ShardPrice = shopItem.Price
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

                foreach (var locked in adventure.Locks)
                {
                    model.Locks.Add(new Models.LockModel
                    {
                        LockedId = locked.LockedId
                    });
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "AdventureCreator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.AdventureModel adventureModel)
        {
            bool newItems = false;
            foreach (var room in adventureModel.Rooms)
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
            if (adventureModel.NewRooms > 0)
            {
                while (adventureModel.NewRooms > 0)
                {
                    adventureModel.Rooms.Add(new Models.RoomModel());
                    adventureModel.NewRooms--;
                }

                newItems = true;
            }

            if (newItems)
                return View(adventureModel);

            if (adventureModel.Rooms == null)
            {
                adventureModel.Rooms = new List<Models.RoomModel>();
            }
            if (adventureModel.Locks == null)
            {
                adventureModel.Locks = new List<Models.LockModel>();
            }
            foreach (var room in adventureModel.Rooms)
            {
                if (room.ShopItems == null)
                    room.ShopItems = new List<Models.ShopItemModel>();

                if (room.Enemies == null)
                    room.Enemies = new List<Models.EnemyModel>();
            }

            if (ModelState.IsValid)
            {
                adventureModel.Rooms?.RemoveAll(r => r.Enemies.Count == 0 && r.ShopItems.Count == 0
                && r.Type != DataModels.Adventures.RoomType.Exit && r.Type != DataModels.Adventures.RoomType.Rest);

                if (adventureModel.Rooms?.FirstOrDefault(r => r.Type == DataModels.Adventures.RoomType.Exit) == null)
                {
                    Alert(AlertMessage.AlertType.Danger, $"No exit room in the dungeon. You must add an exit as the last room you want the player to leave.", "Exit needed !");
                    return View(adventureModel);
                }

                if (adventureModel.Rooms.Count > 0)
                {
                    var adventure = new DataModels.Adventures.Adventure
                    {
                        Id = adventureModel.Id,
                        VersionId = adventureModel.SelectedVersion,
                        Name = adventureModel.Name,
                        RequiredLevel = adventureModel.RequiredLevel,
                        MaxLevelAuthorized = adventureModel.MaxLevelAuthorized,
                        Rooms = new List<DataModels.Adventures.Room>(),
                        ExperienceReward = adventureModel.ExperienceReward,
                        ShardReward = adventureModel.ShardReward,
                        Locks = new List<DataModels.Adventures.Lock>()
                    };

                    foreach (var roomModel in adventureModel.Rooms)
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
                                Price = shopItemModel.ShardPrice
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

                        adventure.Rooms.Add(room);
                    }

                    foreach (var lockModel in adventureModel.Locks)
                    {
                        var locked = new DataModels.Adventures.Lock
                        {
                            Id = Guid.NewGuid(),
                            AdventureId = adventureModel.AdventureId,
                            LockedId = lockModel.LockedId
                        };
                        adventure.Locks.Add(locked);
                    }

                    if (DataRepositories.AdventureRepository.Update(adventure))
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            return View(adventureModel);
        }
    }
}