using Photon.Deterministic;
using Quantum.Collections;
using Quantum.Types;
using System.Linq;

namespace Quantum
{
    public unsafe class MatchSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            if (f.Global->IsMatchRunning)
            {
                if (f.TryFindAsset(f.Global->CurrentMatch.Ruleset.Match.WinCondition.Id, out WinCondition winCondition) &&
                    f.TryFindAsset(f.Global->CurrentMatch.Ruleset.Match.TieResolver.Id, out TieResolver tieResolver))
                {
                    var teams = f.ResolveList(f.Global->Teams);

                    if (winCondition.IsMatchOver(f, teams))
                        EndOfMatch(f, teams, winCondition, tieResolver);
                }
            }
        }

        public override void OnEnabled(Frame f)
        {
            f.Global->CurrentMatch = default;
            f.Global->Teams = f.AllocateList<Team>();
            f.Global->IsTimerOver = false;
            f.Global->IsMatchRunning = false;
            f.Global->CurrentStage = default;

            f.Global->PlayersReady = 0;
            f.Global->TotalPlayers = 0;
            f.Global->CanPlayersEdit = true;

            f.Global->GizmoInstances = f.AllocateList<EntityRef>();
        }

        public override void OnDisabled(Frame f)
        {
            f.FreeList(f.Global->GizmoInstances);
            f.Global->GizmoInstances = default;

            f.FreeList(f.Global->Teams);
            f.Global->Teams = default;
        }

        public static void StartOfMatch(Frame f)
        {
            f.Global->CanPlayersEdit = false;

            StatsSystem.SetAllHealth(f, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);
            StatsSystem.SetAllEnergy(f, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy / 5);
            StatsSystem.SetAllStocks(f, f.Global->CurrentMatch.Ruleset.Players.StockCount);
            PlayerStatsSystem.SetAllShowReadiness(f, false);

            f.SystemEnable<CharacterControllerSystem>();
            f.SystemEnable<ItemSpawnSystem>();

            var teams = f.ResolveList(f.Global->Teams);
            ArrayTeams teamsArray = teams.Count switch
            {
                1 => new() { Item1 = teams[0] },
                2 => new() { Item1 = teams[0], Item2 = teams[1] },
                3 => new() { Item1 = teams[0], Item2 = teams[1], Item3 = teams[2] },
                4 => new() { Item1 = teams[0], Item2 = teams[1], Item3 = teams[2], Item4 = teams[3] },
                _ => default
            };

            f.Events.OnMatchStart(new() { Count = teams.Count, Teams = teamsArray });

            Item item = f.FindAsset<Item>(f.Global->CurrentMatch.Ruleset.Items.StartingItem.Id);
            if (item is not null && item.Prototype.Id.IsValid)
            {
                foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
                {
                    ItemSpawnSettings settings = new()
                    {
                        Item = f.Global->CurrentMatch.Ruleset.Items.StartingItem,
                        Velocity = FPVector2.Zero,
                        Offset = FPVector2.Zero,
                        StartHolding = true
                    };

                    EntityRef newItem = ItemSpawnSystem.SpawnParented(f, settings, stats.Entity);
                }
            }

            if (f.Global->CurrentMatch.Ruleset.Items.SpawnFrequency > 0)
            {
                if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                {
                    itemSpawner->TimeSinceLastSpawned = f.Global->RngSession.Next(itemSpawner->MinTimeToSpawn, itemSpawner->MaxTimeToSpawn) * (1 / f.Global->CurrentMatch.Ruleset.Items.SpawnFrequency);
                }
            }

            f.Global->IsMatchRunning = true;
        }

        public static void EndOfMatch(Frame f, QList<Team> teams, WinCondition winCondition, TieResolver tieResolver)
        {
            f.Global->DeltaTime = (FP._1 / f.UpdateRate) * FP._0_25;
            f.Global->IsMatchRunning = false;

            f.SystemDisable<CharacterControllerSystem>();
            f.SystemDisable<ItemSpawnSystem>();

            ItemSpawnSystem.DespawnAll(f);

            IOrderedEnumerable<Team> winners = teams.OrderBy(winCondition.SortTeams(f, teams)).ThenBy(tieResolver.ResolveTie(f, teams));

            MatchResults results = new()
            {
                Count = winners.Count()
            };

            if (results.Count > 0)
                results.Teams[0] = winners.ElementAt(0);
            
            if (results.Count > 1)
                results.Teams[1] = winners.ElementAt(1);
            
            if (results.Count > 2)
                results.Teams[2] = winners.ElementAt(2);
            
            if (results.Count > 3)
                results.Teams[3] = winners.ElementAt(3);

            PlayerStatsSystem.SetAllShowReadiness(f, true);
            PlayerStatsSystem.SetAllReadiness(f, false);

            f.Global->Results = results;
            f.Events.OnMatchEnd(results);
        }

        public static void SetStage(Frame f, Stage stage)
        {
            Stage old = default;

            if (f.TryFindAsset(f.RuntimeConfig.StageSourceMap.Id, out Map map))
            {
                old = f.Global->CurrentMatch.Stage;
                f.Global->CurrentMatch.Stage = stage;

                if (f.Global->CurrentStage.IsValid)
                {
                    f.Destroy(f.Global->CurrentStage);
                    f.DisposeAsset(f.Map.Guid);

                    var gizmoInstances = f.ResolveList(f.Global->GizmoInstances);
                    while (gizmoInstances.Count > 0)
                    {
                        f.Destroy(gizmoInstances[0]);
                        gizmoInstances.RemoveAt(0);
                    }
                }

                f.Global->CurrentStage = f.Create(stage.Objects.Stage);

                Map newMap = MapGenerator.GenerateMapFromStage(stage, f.Context.AssetSerializer, f.FindAsset<Map>(f.RuntimeConfig.StageSourceMap.Id));

                f.AddAsset(newMap);
                f.Map = newMap;

                for (int i = 0; i < 16; ++i)
                {
                    PositionalGizmo gizmo = ArrayHelper.All(stage.Objects.Gizmos)[i];
                    if (!gizmo.Gizmo.Id.IsValid)
                        continue;

                    Gizmo gizmoAsset = f.FindAsset<Gizmo>(gizmo.Gizmo.Id);
                    EntityRef newGizmo = f.Create(gizmoAsset.Prototype);

                    if (f.Unsafe.TryGetPointer(newGizmo, out Transform2D* transform))
                    {
                        transform->Position = gizmo.Position;
                    }

                    var gizmoInstances = f.ResolveList(f.Global->GizmoInstances);
                    gizmoInstances.Add(newGizmo);
                }
            }

            f.Events.OnStageSelect(f.Global->CurrentStage, old, stage);
        }

        public static void SetRuleset(Frame f, Ruleset ruleset)
        {
            Ruleset old = default;

            old = f.Global->CurrentMatch.Ruleset;
            f.Global->CurrentMatch.Ruleset = ruleset;

            TimerSystem.SetTime(f, new(0, 0, ruleset.Match.Time), true);
            f.Events.OnRulesetSelect(old, ruleset);
        }
    }
}
