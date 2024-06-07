using Photon.Deterministic;
using Quantum.Collections;
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
                StatsSystem.SetAllHealth(f, matchInstance->Match.Ruleset.Players.MaxHealth);
                StatsSystem.SetAllEnergy(f, matchInstance->Match.Ruleset.Players.MaxEnergy / 5);
                StatsSystem.SetAllStocks(f, matchInstance->Match.Ruleset.Players.StockCount);

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
                        EntityRef newItem = ItemSpawnSystem.SpawnInHand(f, matchInstance->Match.Ruleset.Items.StartingItem, stats.Entity);
                    }
                }

                if (matchInstance->Match.Ruleset.Items.SpawnFrequency > 0)
                {
                    if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                    {
                        itemSpawner->TimeSinceLastSpawned = f.Global->RngSession.Next(itemSpawner->MinTimeToSpawn, itemSpawner->MaxTimeToSpawn) * (1 / matchInstance->Match.Ruleset.Items.SpawnFrequency);
                    }
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

        public static void SetStage(Frame f, Stage stage)
        {
            Stage old = default;

            if (f.TryFindAsset(stage.Objects.Map.Id, out Map map))
            {
                if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
                {
                    old = matchInstance->Match.Stage;
                    matchInstance->Match.Stage = stage;

                    if (matchInstance->CurrentStage.IsValid)
                        f.Destroy(matchInstance->CurrentStage);
                    
                    matchInstance->CurrentStage = f.Create(stage.Objects.Stage);
                }

                f.Map = map;
            }

            f.Events.OnStageSelect(old, stage);
        }

        public static void SetRuleset(Frame f, Ruleset ruleset)
        {
            Ruleset old = default;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                old = matchInstance->Match.Ruleset;
                matchInstance->Match.Ruleset = ruleset;
            }

            TimerSystem.SetTime(f, new(0, 0, ruleset.Match.Time));
            f.Events.OnRulesetSelect(old, ruleset);
        }
    }
}
