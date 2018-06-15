﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class MonstersController : Controller
    {
        public ActionResult Index(Models.Filters.MonsterFilter filter)
        {
            var monsters = DataRepositories.MonsterRepository.GetAll();

            filter.VersionList = DataRepositories.VersionRepository.GetAll();
            filter.Monsters = filter.FilterList(monsters);

            return View(filter);
        }

        public ActionResult Details(Guid id)
        {
            var monster = DataRepositories.MonsterRepository.GetById(id);

            return View(monster);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult Create()
        {
            var monster = new Models.MonsterModel();
            monster.VersionList = DataRepositories.VersionRepository.GetAll();
            monster.Loots = new List<Models.LootModel>();
            monster.Phases = new List<Models.PhaseModel>();

            return View(monster);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.MonsterModel monsterModel)
        {
            if (monsterModel.Loots == null)
            {
                monsterModel.Loots = new List<Models.LootModel>();
            }
            if (monsterModel.Phases == null)
            {
                monsterModel.Phases = new List<Models.PhaseModel>();
            }

            if (ModelState.IsValid)
            {
                monsterModel.Types = monsterModel.Types.Distinct().ToList();
                monsterModel.Loots.RemoveAll(l => l.DropRate == 0 || l.Quantity == 0 || Guid.Empty.Equals(l.SelectedItem));
                monsterModel.Phases.RemoveAll(p => Guid.Empty.Equals(p.SelectedSkill));

                if (monsterModel.Types.Count > 0 && monsterModel.Phases.Count > 0)
                {
                    var monsterId = Guid.NewGuid();
                    var monster = new DataModels.Monsters.Monster
                    {
                        Id = Guid.NewGuid(),
                        VersionId = monsterModel.SelectedVersion,
                        MonsterId = monsterId,
                        Name = monsterModel.Name,
                        Description = monsterModel.Description,
                        Story = monsterModel.Story,
                        Image = monsterModel.Image,
                        Types = monsterModel.Types,
                        HealthPerLevel = monsterModel.HealthPerLevel,
                        BaseExperience = monsterModel.BaseExperience,
                        ExperiencePerLevelRatio = monsterModel.ExperiencePerLevelRatio,
                        StaminaPerLevel = monsterModel.StaminaPerLevel,
                        EnergyPerLevel = monsterModel.EnergyPerLevel,
                        StrengthPerLevel = monsterModel.StrengthPerLevel,
                        AgilityPerLevel = monsterModel.AgilityPerLevel,
                        IntelligencePerLevel = monsterModel.IntelligencePerLevel,
                        WisdomPerLevel = monsterModel.WisdomPerLevel,
                        Loots = new List<DataModels.Monsters.Loot>(),
                        Phases = new List<DataModels.Monsters.Phase>()
                    };

                    foreach (var lootModel in monsterModel.Loots)
                    {
                        var loot = new DataModels.Monsters.Loot
                        {
                            Id = Guid.NewGuid(),
                            MonsterId = monsterId,
                            Type = lootModel.Type,
                            ItemId = lootModel.SelectedItem,
                            DropRate = lootModel.DropRate,
                            Quantity = lootModel.Quantity
                        };
                        monster.Loots.Add(loot);
                    }

                    foreach (var phaseModel in monsterModel.Phases)
                    {
                        var phase = new DataModels.Monsters.Phase
                        {
                            Id = Guid.NewGuid(),
                            MonsterId = monsterId,
                            SkillId = phaseModel.SelectedSkill
                        };
                        monster.Phases.Add(phase);
                    }

                    if (DataRepositories.MonsterRepository.Create(monster))
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            monsterModel.VersionList = DataRepositories.VersionRepository.GetAll();
            return View(monsterModel);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddTypePartial()
        {
            return PartialView("TypePartial");
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddLootPartial()
        {
            var loot = new Models.LootModel();

            return PartialView("LootPartial", loot);
        }

        [CustomAuthorize(Roles = "SuperAdmin")]
        [HttpGet]
        public ActionResult AddPhasePartial()
        {
            var phase = new Models.PhaseModel();

            return PartialView("PhasePartial", phase);
        }
    }
}