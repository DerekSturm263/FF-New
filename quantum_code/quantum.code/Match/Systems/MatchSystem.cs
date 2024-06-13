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
                if (f.TryFindAsset(f.Global->CurrentMatch.Ruleset.Match.WinCondition.Id, out WinCondition winCondition))
                {
                    var teams = f.ResolveList(f.Global->Teams);

                    if (winCondition.IsMatchOver(f, teams))
                        EndOfMatch(f, teams, winCondition);
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
        }

        public override void OnDisabled(Frame f)
        {
            f.FreeList(f.Global->Teams);
            f.Global->Teams = default;
        }

        public static void StartOfMatch(Frame f)
        {
            f.Global->CanPlayersEdit = false;

            StatsSystem.SetAllHealth(f, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);
            StatsSystem.SetAllEnergy(f, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy / 5);
            StatsSystem.SetAllStocks(f, f.Global->CurrentMatch.Ruleset.Players.StockCount);

            f.SystemEnable<CharacterControllerSystem>();
            f.SystemEnable<ItemSpawnSystem>();

            var teams = f.ResolveList(f.Global->Teams);

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
                if (f.Global->CurrentMatch.Ruleset.Items.StartingItem.Id != AssetGuid.Invalid)
                {
                    EntityRef newItem = ItemSpawnSystem.SpawnInHand(f, f.Global->CurrentMatch.Ruleset.Items.StartingItem, stats.Entity);
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

        public static void EndOfMatch(Frame f, QList<Team> teams, WinCondition winCondition)
        {
            f.Global->DeltaTime = (FP._1 / f.UpdateRate) * FP._0_25;
            f.Global->IsMatchRunning = false;

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

        public static void SetStage(Frame f, Stage stage)
        {
            Stage old = default;

            if (f.TryFindAsset(stage.Objects.Map.Id, out Map map))
            {
                old = f.Global->CurrentMatch.Stage;
                f.Global->CurrentMatch.Stage = stage;

                if (f.Global->CurrentStage.IsValid)
                    f.Destroy(f.Global->CurrentStage);

                f.Global->CurrentStage = f.Create(stage.Objects.Stage);

                f.Map = map;
            }

            foreach (var stats in f.Unsafe.GetComponentBlockIterator<Stats>())
            {
                Transform2D* transform = f.Unsafe.GetPointer<Transform2D>(stats.Entity);
                transform->Position = ArrayHelper.Get(stage.Spawn.PlayerSpawnPoints, stats.Component->GlobalIndex);
            }

            f.Events.OnStageSelect(old, stage);
        }

        public static void SetRuleset(Frame f, Ruleset ruleset)
        {
            Ruleset old = default;

            old = f.Global->CurrentMatch.Ruleset;
            f.Global->CurrentMatch.Ruleset = ruleset;

            TimerSystem.SetTime(f, new(0, 0, ruleset.Match.Time));
            f.Events.OnRulesetSelect(old, ruleset);
        }
    }
}
