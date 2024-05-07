using Photon.Deterministic;
using Quantum.Collections;
using System.ComponentModel;
using System.Linq;

namespace Quantum
{
    public unsafe class MatchSystem : SystemMainThreadFilter<MatchSystem.Filter>, ISignalOnComponentAdded<MatchInstance>, ISignalOnComponentRemoved<MatchInstance>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public MatchInstance* MatchInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.MatchInstance->IsMatchRunning)
            {
                if (f.TryFindAsset(filter.MatchInstance->Match.Ruleset.Match.WinCondition.Id, out WinCondition winCondition))
                {
                    var teams = f.ResolveList(filter.MatchInstance->Match.Teams);

                    if (winCondition.IsMatchOver(f, teams))
                        EndOfMatch(f, teams, winCondition);
                }
            }
        }

        public static void StartOfMatch(Frame f)
        {
            if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
            {
                playerCounter->CanPlayersEdit = false;
            }

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                var filter = f.Unsafe.FilterStruct<StatsSystem.PlayerLinkStatsFilter>();
                var playerLinkStats = default(StatsSystem.PlayerLinkStatsFilter);

                while (filter.Next(&playerLinkStats))
                {
                    StatsSystem.SetHealth(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, matchInstance->Match.Ruleset.Players.MaxHealth);
                    StatsSystem.SetEnergy(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, matchInstance->Match.Ruleset.Players.MaxEnergy / 5);
                    StatsSystem.SetStocks(f, playerLinkStats.PlayerLink, playerLinkStats.Stats, matchInstance->Match.Ruleset.Players.StockCount);
                }

                f.SystemEnable<CharacterControllerSystem>();
                f.SystemEnable<ItemSpawnSystem>();

                var teams = f.ResolveList(matchInstance->Match.Teams);

                switch (teams.Count)
                {
                    case 1:
                        f.Events.OnMatchStart(teams[0], default, default, default);
                        break;

                    case 2:
                        f.Events.OnMatchStart(teams[0], teams[1], default, default);
                        break;

                    case 3:
                        f.Events.OnMatchStart(teams[0], teams[1], teams[2], default);
                        break;

                    case 4:
                        f.Events.OnMatchStart(teams[0], teams[1], teams[2], teams[3]);
                        break;
                }

                foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
                {
                    if (matchInstance->Match.Ruleset.Items.StartingItem.Id != AssetGuid.Invalid)
                    {
                        EntityRef newItem = ItemSpawnSystem.SpawnItem(f, matchInstance->Match.Ruleset.Items.StartingItem, FPVector2.Zero);
                        ItemSystem.PickUp(f, stats.Entity, newItem);
                    }
                }

                if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                {
                    itemSpawner->TimeSinceLastSpawned = f.Global->RngSession.Next(itemSpawner->MinTimeToSpawn, itemSpawner->MaxTimeToSpawn) * (1 / matchInstance->Match.Ruleset.Items.SpawnFrequency);
                }
            }

            matchInstance->IsMatchRunning = true;
        }
        public static void EndOfMatch(Frame f, QList<Team> teams, WinCondition winCondition)
        {
            f.Global->DeltaTime = (FP._1 / f.UpdateRate) * FP._0_25;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                matchInstance->IsMatchRunning = false;
            }

            f.SystemDisable<CharacterControllerSystem>();
            f.SystemDisable<ItemSpawnSystem>();

            IOrderedEnumerable<Team> winners = teams.OrderBy(winCondition.SortTeams(f, teams));

            switch (teams.Count)
            {
                case 1:
                    f.Events.OnMatchEnd(winners.ElementAt(0), default, default, default, false);
                    break;

                case 2:
                    f.Events.OnMatchEnd(winners.ElementAt(0), winners.ElementAt(1), default, default, false);
                    break;

                case 3:
                    f.Events.OnMatchEnd(winners.ElementAt(0), winners.ElementAt(1), winners.ElementAt(2), default, false);
                    break;

                case 4:
                    f.Events.OnMatchEnd(winners.ElementAt(0), winners.ElementAt(1), winners.ElementAt(2), winners.ElementAt(3), false);
                    break;
            }
        }

        public void OnAdded(Frame f, EntityRef entity, MatchInstance* component)
        {
            component->Match.Teams = f.AllocateList<Team>();
        }

        public void OnRemoved(Frame f, EntityRef entity, MatchInstance* component)
        {
            f.FreeList(component->Match.Teams);
            component->Match.Teams = default;
        }
    }
}
