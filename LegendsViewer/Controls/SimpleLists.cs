﻿using System.Collections.Generic;
using System.Linq;
using LegendsViewer.Legends;
using LegendsViewer.Legends.Enums;
using LegendsViewer.Legends.EventCollections;
using LegendsViewer.Legends.Events;

namespace LegendsViewer.Controls
{
    public class WorldObjectList
    {
        internal readonly World World;
        public bool sortEvents, sortFiltered;
        public static int MaxResults = 500;

        protected WorldObjectList(World world)
        {
            World = world;
        }
    }

    public class HistoricalFigureList : WorldObjectList
    {
        public bool deity, vampire, force, werebeast, necromancer, animated, ghost, alive, Leader, sortKills, sortBattles, sortMiscKills;
        public string name, race, caste, type;
        public readonly List<HistoricalFigure> BaseList;

        public HistoricalFigureList(World setWorld) : base(setWorld)
        {
            BaseList = World.HistoricalFigures;
        }
        public IEnumerable<HistoricalFigure> GetList()
        {
            IEnumerable<HistoricalFigure> filtered = BaseList;
            if (name != "") filtered = filtered.Where(hf => hf.Name.ToLower().Contains(name.ToLower()));
            if (race != "All") filtered = filtered.Where(hf => hf.Race == race);
            if (caste != "All") filtered = filtered.Where(hf => hf.Caste == caste);
            if (type != "All") filtered = filtered.Where(hf => hf.AssociatedType == type);
            if (deity) filtered = filtered.Where(hf => hf.Deity);
            if (vampire) filtered = filtered.Where(hf => hf.ActiveInteractions.Any(x => x.Contains("VAMPIRE")));
            if (werebeast) filtered = filtered.Where(hf => hf.ActiveInteractions.Any(x => x.Contains("WEREBEAST")));
            if (necromancer) filtered = filtered.Where(hf => hf.ActiveInteractions.Any(x => x.Contains("SECRET") && !x.Contains("ANIMATE")));
            if (animated) filtered = filtered.Where(hf => hf.ActiveInteractions.Any(x => x.Contains("ANIMATE")));
            if (force) filtered = filtered.Where(hf => hf.Force);
            if (ghost) filtered = filtered.Where(hf => hf.Ghost);
            if (Leader) filtered = filtered.Where(hf => hf.Positions.Count > 0);
            if (alive) filtered = filtered.Where(hf => hf.DeathYear == -1);
            if (sortKills) filtered = filtered.OrderByDescending(hf => hf.NotableKills.Count);
            if (sortEvents) filtered = filtered.OrderByDescending(hf => hf.Events.Count);
            if (sortMiscKills) filtered = filtered.OrderByDescending(hf => hf.Reputations.Sum(ev => ev.UnsolvedMurders));
            if (sortFiltered) filtered = filtered.OrderByDescending(hf => hf.Events.Count(ev => !HistoricalFigure.Filters.Contains(ev.Type)));
            if (sortBattles) filtered = filtered.OrderByDescending(hf => hf.Battles.Count(battle => !World.FilterBattles || battle.Notable));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class SitesList : WorldObjectList
    {
        public string name, type, PopulationType;
        public bool sortOwners, sortWarfare, SortPopulation, SortConnections, SortDeaths, SortBeastAttacks;
        public readonly List<Site> BaseList;
        public SitesList(World setWorld) : base(setWorld)
        {
            BaseList = World.Sites.Where(site => !string.IsNullOrWhiteSpace(site.Name)).ToList();
        }
        public IEnumerable<Site> getList()
        {
            IEnumerable<Site> filtered = BaseList;
            if (name != "") filtered = filtered.Where(s => s.Name.ToLower().Contains(name.ToLower()) || s.UntranslatedName.ToLower().Contains(name.ToLower()));
            if (type != "All") filtered = filtered.Where(s => s.Type == type);
            if (sortOwners) filtered = filtered.OrderByDescending(s => s.OwnerHistory.Count());
            if (sortEvents) filtered = filtered.OrderByDescending(s => s.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(s => s.Events.Count(ev => !Site.Filters.Contains(ev.Type)));
            if (sortWarfare) filtered = filtered.OrderByDescending(site => site.Warfare.OfType<Battle>().Count(battle => !World.FilterBattles || battle.Notable));
            if (SortPopulation)
                if (PopulationType != "All") filtered = filtered.Where(site => site.Populations.Count(population => population.Race == PopulationType) > 0).OrderByDescending(site => site.Populations.Where(population => population.Race == PopulationType).Sum(population => population.Count));
                else filtered = filtered.OrderByDescending(site => site.Populations.Sum(population => population.Count));
            if (SortConnections) filtered = filtered.OrderByDescending(site => site.Connections.Count);
            if (SortDeaths) filtered = filtered.OrderByDescending(site => site.Events.OfType<HFDied>().Count() + site.Warfare.OfType<Battle>().Sum(battle => battle.AttackerSquads.Sum(squad => squad.Deaths) + battle.DefenderSquads.Sum(squad => squad.Deaths)));
            if (SortBeastAttacks) filtered = filtered.OrderByDescending(site => site.BeastAttacks.Count);
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class RegionsList : WorldObjectList
    {
        public string name, type;
        public bool sortBattles, SortDeaths, SortArea;
        public readonly List<WorldRegion> BaseList;
        public RegionsList(World setWorld) : base(setWorld)
        {
            BaseList = World.Regions;
        }
        public IEnumerable<WorldRegion> getList()
        {
            IEnumerable<WorldRegion> filtered = BaseList;
            if (name != "") filtered = filtered.Where(r => r.Name.ToLower().Contains(name.ToLower()));
            if (type != "All") filtered = filtered.Where(r => r.Type == type);
            if (sortEvents) filtered = filtered.OrderByDescending(r => r.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(r => r.Events.Count(ev => !WorldRegion.Filters.Contains(ev.Type)));
            if (sortBattles) filtered = filtered.OrderByDescending(region => region.Battles.Count(battle => !World.FilterBattles || battle.Notable));
            if (SortDeaths) filtered = filtered.OrderByDescending(region => region.Events.OfType<HFDied>().Count() + region.Battles.OfType<Battle>().Sum(battle => battle.AttackerSquads.Sum(squad => squad.Deaths) + battle.DefenderSquads.Sum(squad => squad.Deaths)));
            if (SortArea) filtered = filtered.OrderByDescending(region => region.SquareTiles);
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class UndergroundRegionsList : WorldObjectList
    {
        public string type;
        public readonly List<UndergroundRegion> BaseList;
        public bool SortArea;
        public UndergroundRegionsList(World setWorld) : base(setWorld)
        {
            BaseList = World.UndergroundRegions;
        }
        public IEnumerable<UndergroundRegion> getList()
        {
            IEnumerable<UndergroundRegion> filtered = BaseList;
            if (type != "All") filtered = filtered.Where(ur => ur.Type == type);
            if (sortEvents) filtered = filtered.OrderByDescending(ur => ur.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(ur => ur.Events.Count(ev => !UndergroundRegion.Filters.Contains(ev.Type)));
            if (SortArea) filtered = filtered.OrderByDescending(region => region.SquareTiles);
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class LandmassesList : WorldObjectList
    {
        public string Name;
        public readonly List<Landmass> BaseList;
        public LandmassesList(World setWorld) : base(setWorld)
        {
            BaseList = World.Landmasses;
        }
        public IEnumerable<Landmass> getList()
        {
            IEnumerable<Landmass> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(element => element.Name.ToLower().Contains(Name.ToLower()));
            if (sortEvents) filtered = filtered.OrderByDescending(element => element.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(element => element.Events.Count(ev => !Landmass.Filters.Contains(ev.Type)));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class MountainPeaksList : WorldObjectList
    {
        public string Name;
        public readonly List<MountainPeak> BaseList;
        public MountainPeaksList(World setWorld) : base(setWorld)
        {
            BaseList = World.MountainPeaks;
        }
        public IEnumerable<MountainPeak> getList()
        {
            IEnumerable<MountainPeak> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(element => element.Name.ToLower().Contains(Name.ToLower()));
            if (sortEvents) filtered = filtered.OrderByDescending(element => element.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(element => element.Events.Count(ev => !MountainPeak.Filters.Contains(ev.Type)));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class EntitiesList : WorldObjectList
    {
        public string Type;
        public string name, race, PopulationType;
        public bool sortSites, civs, sortWars, SortPopulation;
        public readonly List<Entity> BaseList;
        public EntitiesList(World setWorld) : base(setWorld)
        {
            BaseList = World.Entities;
        }
        public IEnumerable<Entity> getList()
        {
            IEnumerable<Entity> filtered = BaseList.Where(entity => entity.Name != "");
            if (name != "") filtered = filtered.Where(e => e.Name.ToLower().Contains(name.ToLower()));
            if (Type != "All") filtered = filtered.Where(element => element.Type.GetDescription() == Type);
            if (race != "All") filtered = filtered.Where(e => e.Race == race);
            if (civs) filtered = filtered.Where(e => e.IsCiv);
            if (sortSites) filtered = filtered.OrderByDescending(civ => civ.SiteHistory.Count);
            if (sortEvents) filtered = filtered.OrderByDescending(e => e.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(e => e.Events.Count(ev => !Entity.Filters.Contains(ev.Type)));
            if (sortWars) filtered = filtered.OrderByDescending(entity => entity.Wars.Count(war => !World.FilterBattles || war.Notable));
            if (SortPopulation)
                if (PopulationType != "All") filtered = filtered.Where(entity => entity.Populations.Count(population => population.Race == PopulationType) > 0).OrderByDescending(civ => civ.Populations.Where(population => population.Race == PopulationType).Sum(population => population.Count));
                else filtered = filtered.OrderByDescending(civ => civ.Populations.Sum(population => population.Count));

            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class WarsList : WorldObjectList
    {
        public string Name;
        public bool SortLength, SortDeaths, Ongoing, SortWarfare, SortConquerings;
        public List<War> BaseList;
        public WarsList(World setWorld) : base(setWorld)
        {
            BaseList = World.EventCollections.OfType<War>().ToList();
        }
        public IEnumerable<War> GetList()
        {
            IEnumerable<War> filtered = BaseList; // BaseList.Where(war => !World.FilterBattles || war.Notable);
            if (Name != "") filtered = filtered.Where(war => war.Name.ToLower().Contains(Name.ToLower()));
            if (Ongoing) filtered = filtered.Where(war => war.EndYear == -1);
            if (sortEvents) filtered = filtered.OrderByDescending(war => war.GetSubEvents().Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(war => war.GetSubEvents().Count(ev => !War.Filters.Contains(ev.Type)));
            if (SortLength) filtered = filtered.OrderByDescending(war => war.Length);
            if (SortDeaths) filtered = filtered.OrderByDescending(war => war.DeathCount);
            if (SortWarfare) filtered = filtered.OrderByDescending(war => war.Collections.Count);
            if (SortConquerings) filtered = filtered.OrderByDescending(war => war.Collections.OfType<SiteConquered>().Count(conquering => conquering.ConquerType != SiteConqueredType.Pillaging));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class BattlesList : WorldObjectList
    {
        public string Name;
        public bool SortDeaths;
        public List<Battle> BaseList;
        public BattlesList(World setWorld) : base(setWorld)
        {
            BaseList = World.EventCollections.OfType<Battle>().ToList();
        }
        public IEnumerable<Battle> GetList()
        {
            IEnumerable<Battle> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(battle => battle.Name.ToLower().Contains(Name.ToLower()));
            if (sortEvents) filtered = filtered.OrderByDescending(battle => battle.GetSubEvents().Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(battle => battle.GetSubEvents().Count(ev => !Battle.Filters.Contains(ev.Type)));
            if (SortDeaths) filtered = filtered.OrderByDescending(battle => battle.AttackerDeathCount + battle.DefenderDeathCount);
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class ArtifactsList : WorldObjectList
    {
        public string Name, Type, Material;
        public readonly List<Artifact> BaseList;
        public ArtifactsList(World setWorld) : base(setWorld)
        {
            BaseList = World.Artifacts;
        }
        public IEnumerable<Artifact> GetList()
        {
            IEnumerable<Artifact> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(artifact => artifact.Name.ToLower().Contains(Name.ToLower()));
            if (!string.IsNullOrEmpty(Type) && Type != "All") filtered = filtered.Where(artifact => artifact.Type.ToLower().Contains(Type.ToLower()));
            if (!string.IsNullOrEmpty(Material)) filtered = filtered.Where(artifact => artifact.Material.ToLower().Contains(Material.ToLower()));
            if (sortEvents) filtered = filtered.OrderByDescending(artifact => artifact.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(artifact => artifact.FilteredEvents.Count(ev => !Artifact.Filters.Contains(ev.Type)));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class WrittenContentList : WorldObjectList
    {
        public string Name, Type;
        public readonly List<WrittenContent> BaseList;
        public WrittenContentList(World setWorld) : base(setWorld)
        {
            BaseList = World.WrittenContents;
        }
        public IEnumerable<WrittenContent> GetList()
        {
            IEnumerable<WrittenContent> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(element => element.Name.ToLower().Contains(Name.ToLower()));
            if (Type != "All") filtered = filtered.Where(element => element.Type.GetDescription() == Type);
            if (sortEvents) filtered = filtered.OrderByDescending(element => element.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(element => element.FilteredEvents.Count(ev => !WrittenContent.Filters.Contains(ev.Type)));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class WorldConstructionsList : WorldObjectList
    {
        public string Name, Type;
        public readonly List<WorldConstruction> BaseList;
        public WorldConstructionsList(World setWorld) : base(setWorld)
        {
            BaseList = World.WorldConstructions; }
        public IEnumerable<WorldConstruction> GetList()
        {
            IEnumerable<WorldConstruction> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(element => element.Name.ToLower().Contains(Name.ToLower()));
            if (Type != "All") filtered = filtered.Where(element => element.Type.GetDescription() == Type);
            if (sortEvents) filtered = filtered.OrderByDescending(element => element.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(element => element.FilteredEvents.Count(ev => !WorldConstruction.Filters.Contains(ev.Type)));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class StructuresList : WorldObjectList
    {
        public string Name, Type;
        public readonly List<Structure> BaseList;
        public StructuresList(World setWorld) : base(setWorld)
        {
            BaseList = World.Structures;
        }
        public IEnumerable<Structure> GetList()
        {
            IEnumerable<Structure> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(element => element.Name.ToLower().Contains(Name.ToLower()));
            if (Type != "All") filtered = filtered.Where(element => element.Type.GetDescription() == Type);
            if (sortEvents) filtered = filtered.OrderByDescending(element => element.Events.Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(element => element.FilteredEvents.Count(ev => !Structure.Filters.Contains(ev.Type)));
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class ConqueringsList : WorldObjectList
    {
        public string Name, Type;
        public bool SortSite;
        public readonly List<SiteConquered> BaseList;
        public ConqueringsList(World setWorld) : base(setWorld)
        {
            BaseList = World.EventCollections.OfType<SiteConquered>().ToList();
        }
        public IEnumerable<SiteConquered> GetList()
        {
            IEnumerable<SiteConquered> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(pillaging => pillaging.Name.ToLower().Contains(Name.ToLower()));
            if (Type != "All") filtered = filtered.Where(conquering => conquering.ConquerType.ToString() == Type);
            if (sortEvents) filtered = filtered.OrderByDescending(pillaging => pillaging.GetSubEvents().Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(pillaging => pillaging.GetSubEvents().Count(ev => !SiteConquered.Filters.Contains(ev.Type)));
            if (SortSite) filtered = filtered.OrderBy(conquering => conquering.Site.ToString());
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }

    public class BeastAttackList : WorldObjectList
    {
        public string Name;
        public bool SortDeaths;
        public readonly List<BeastAttack> BaseList;
        public BeastAttackList(World setWorld) : base(setWorld)
        {
            BaseList = World.EventCollections.OfType<BeastAttack>().ToList();
        }
        public IEnumerable<BeastAttack> GetList()
        {
            IEnumerable<BeastAttack> filtered = BaseList;
            if (Name != "") filtered = filtered.Where(beastAttack => beastAttack.Name.ToLower().Contains(Name.ToLower()));
            if (sortEvents) filtered = filtered.OrderByDescending(beastAttack => beastAttack.GetSubEvents().Count);
            if (sortFiltered) filtered = filtered.OrderByDescending(beastAttack => beastAttack.GetSubEvents().Count(ev => !BeastAttack.Filters.Contains(ev.Type)));
            if (SortDeaths) filtered = filtered.OrderByDescending(beastAttack => beastAttack.GetSubEvents().OfType<HFDied>().Count());
            return MaxResults > 0 ? filtered.Take(MaxResults) : filtered;
        }
    }
}
