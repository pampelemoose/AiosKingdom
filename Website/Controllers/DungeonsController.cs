﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class DungeonsController : Controller
    {
        public ActionResult Index(Models.Filters.DungeonFilter filter)
        {
            var dungeons = DataRepositories.DungeonRepository.GetAll();

            filter.VersionList = DataRepositories.VersionRepository.GetAll();
            filter.Dungeons = filter.FilterList(dungeons);

            return View(filter);
        }

        public ActionResult Details(Guid id)
        {
            var dungeon = DataRepositories.DungeonRepository.GetById(id);

            return View(dungeon);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var dungeon = new Models.DungeonModel();
            dungeon.VersionList = DataRepositories.VersionRepository.GetAll();
            dungeon.Rooms = new List<Models.RoomModel>();

            return View(dungeon);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
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
                dungeonModel.Rooms?.RemoveAll(r => r.Enemies.Count == 0 && r.ShopItems.Count == 0);

                if (dungeonModel.Rooms.Count > 0)
                {
                    var dungeonId = Guid.NewGuid();
                    var dungeon = new DataModels.Dungeons.Dungeon
                    {
                        Id = Guid.NewGuid(),
                        VersionId = dungeonModel.SelectedVersion,
                        DungeonId = dungeonId,
                        Name = dungeonModel.Name,
                        RequiredLevel = dungeonModel.RequiredLevel,
                        MaxLevelAuthorized = dungeonModel.RequiredLevel,
                        Rooms = new List<DataModels.Dungeons.Room>()
                    };

                    foreach (var roomModel in dungeonModel.Rooms)
                    {
                        var room = new DataModels.Dungeons.Room
                        {
                            Id = Guid.NewGuid(),
                            DungeonId = dungeonId,
                            Type = roomModel.Type,
                            RoomNumber = roomModel.RoomNumber,
                            ShopItems = new List<DataModels.Dungeons.ShopItem>(),
                            Ennemies = new List<DataModels.Dungeons.Enemy>()
                        };

                        foreach (var shopItemModel in roomModel.ShopItems)
                        {

                        }

                        dungeon.Rooms.Add(room);
                    }

                    /*if (DataRepositories.MonsterRepository.Create(dungeon))
                    {
                        return RedirectToAction("Index");
                    }*/
                }
            }

            dungeonModel.VersionList = DataRepositories.VersionRepository.GetAll();
            return View(dungeonModel);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddRoomPartial()
        {
            var room = new Models.RoomModel();
            room.ShopItems = new List<Models.ShopItemModel>();
            room.Enemies = new List<Models.EnemyModel>();

            return PartialView("RoomPartial", room);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddShopItemPartial(string id)
        {
            var shopItem = new Models.ShopItemModel();
            shopItem.RoomId = id;

            return PartialView("ShopItemPartial", shopItem);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddEnemyPartial(string id)
        {
            var enemy = new Models.EnemyModel();
            enemy.RoomId = id;

            return PartialView("EnemyPartial", enemy);
        }
    }
}