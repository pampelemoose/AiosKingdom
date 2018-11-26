using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website.Authentication;

namespace Website.Controllers
{
    public class MonstersController : AKBaseController
    {
        public ActionResult Index(Models.Filters.MonsterFilter filter)
        {
            var monsters = DataRepositories.MonsterRepository.GetAll();

            filter.Monsters = filter.FilterList(monsters);

            return View(filter);
        }

        public ActionResult Details(Guid id)
        {
            var monster = DataRepositories.MonsterRepository.GetById(id);

            return View(monster);
        }

        [CustomAuthorize(Roles = "MonsterCreator")]
        [HttpGet]
        public ActionResult Create()
        {
            var monster = new Models.MonsterModel();

            monster.Loots = new List<Models.LootModel>();
            monster.Phases = new List<Models.PhaseModel>();

            return View(monster);
        }

        [CustomAuthorize(Roles = "MonsterCreator")]
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
            if (monsterModel.Types == null)
            {
                monsterModel.Types = new List<DataModels.Monsters.MonsterType>();
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
                        Types = monsterModel.Types,
                        BaseHealth = monsterModel.BaseHealth,
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
                else
                {
                    Alert(AlertMessage.AlertType.Danger, $"You need at least one Type and one Phase.", "Type or Phase missing !");
                }
            }

            return View(monsterModel);
        }

        [CustomAuthorize(Roles = "MonsterCreator")]
        [HttpGet]
        public ActionResult AddTypePartial()
        {
            return PartialView("TypePartial");
        }

        [CustomAuthorize(Roles = "MonsterCreator")]
        [HttpGet]
        public ActionResult AddLootPartial()
        {
            var loot = new Models.LootModel();

            return PartialView("LootPartial", loot);
        }

        [CustomAuthorize(Roles = "MonsterCreator")]
        [HttpGet]
        public ActionResult AddPhasePartial(string version)
        {
            var phase = new Models.PhaseModel();
            phase.SelectedVersion = Guid.Parse(version);

            return PartialView("PhasePartial", phase);
        }

        [CustomAuthorize(Roles = "MonsterCreator")]
        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var monster = DataRepositories.MonsterRepository.GetById(id);

            if (monster != null)
            {
                var model = new Models.MonsterModel
                {
                    Id = monster.Id,
                    SelectedVersion = monster.VersionId,
                    Name = monster.Name,
                    Description = monster.Description,
                    Story = monster.Story,
                    Types = monster.Types,
                    BaseHealth = monster.BaseHealth,
                    HealthPerLevel = monster.HealthPerLevel,
                    BaseExperience = monster.BaseExperience,
                    ExperiencePerLevelRatio = monster.ExperiencePerLevelRatio,
                    StaminaPerLevel = monster.StaminaPerLevel,
                    EnergyPerLevel = monster.EnergyPerLevel,
                    StrengthPerLevel = monster.StrengthPerLevel,
                    AgilityPerLevel = monster.AgilityPerLevel,
                    IntelligencePerLevel = monster.IntelligencePerLevel,
                    WisdomPerLevel = monster.WisdomPerLevel,
                    Loots = new List<Models.LootModel>(),
                    Phases = new List<Models.PhaseModel>()
                };

                foreach (var loot in monster.Loots)
                {
                    model.Loots.Add(new Models.LootModel
                    {
                        Id = loot.Id,
                        Type = loot.Type,
                        SelectedItem = loot.ItemId,
                        Quantity = loot.Quantity,
                        DropRate = loot.DropRate
                    });
                }

                foreach (var phase in monster.Phases)
                {
                    model.Phases.Add(new Models.PhaseModel
                    {
                        SelectedVersion = model.SelectedVersion,
                        Id = phase.Id,
                        SelectedSkill = phase.SkillId
                    });
                }

                return View(model);
            }

            return RedirectToAction("Index");
        }

        [CustomAuthorize(Roles = "MonsterCreator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.MonsterModel monsterModel)
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

                if (monsterModel.Types.Count > 0 && monsterModel.Phases.Count > 0)
                {
                    var monster = new DataModels.Monsters.Monster
                    {
                        Id = monsterModel.Id,
                        VersionId = monsterModel.SelectedVersion,
                        Name = monsterModel.Name,
                        Description = monsterModel.Description,
                        Story = monsterModel.Story,
                        Types = monsterModel.Types,
                        BaseHealth = monsterModel.BaseHealth,
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
                            Id = lootModel.Id,
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
                            Id = phaseModel.Id,
                            SkillId = phaseModel.SelectedSkill
                        };
                        monster.Phases.Add(phase);
                    }

                    if (DataRepositories.MonsterRepository.Update(monster))
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    Alert(AlertMessage.AlertType.Danger, $"You need at least one Type and one Phase.", "Type or Phase missing !");
                }
            }

            return View(monsterModel);
        }
    }
}