using Photon.Deterministic;
using System.Reflection;

namespace Quantum
{
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            RuntimePlayer data = f.GetPlayerData(player);
            EntityPrototype prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

            SpawnPlayer(f, player, prototype, true, data.Name, data.Index, data.IsRealBattle, data.Build);
        }

        public static EntityRef SpawnPlayer(Frame f, PlayerRef player, AssetRefEntityPrototype prototype, bool assignLink, string name, FighterIndex index, bool isRealBattle, Build defaultBuild)
        {
            EntityRef entity = f.Create(prototype);

            PlayerLink playerLink = new()
            {
                Player = player,
                Entity = entity
            };

            PlayerStats* playerStats = f.Unsafe.GetPointer<PlayerStats>(entity);
            playerStats->Name = name;
            playerStats->Index = index;

            PlayerStatsSystem.SetBuild(f, entity, playerStats, defaultBuild);
            AddPlayerToList(f, entity, index);

            if (assignLink)
            {
                f.Add(entity, playerLink);
            }

            f.Events.OnPlayerSpawn(new() { Entity = entity, Index = index, Name = name });

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                if (f.Global->CurrentMatch.Ruleset.Match.Time == -1)
                {
                    stats->CurrentStats.Health = f.Global->CurrentMatch.Ruleset.Players.MaxHealth;
                    stats->CurrentStats.Energy = f.Global->CurrentMatch.Ruleset.Players.MaxEnergy;
                    stats->CurrentStats.Stocks = f.Global->CurrentMatch.Ruleset.Players.StockCount;
                }

                f.Events.OnPlayerModifyHealth(entity, index, stats->CurrentStats.Health, stats->CurrentStats.Health, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);
                f.Events.OnPlayerModifyEnergy(entity, index, stats->CurrentStats.Energy, stats->CurrentStats.Energy, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy);
                f.Events.OnPlayerModifyStocks(entity, index, stats->CurrentStats.Stocks, stats->CurrentStats.Stocks, f.Global->CurrentMatch.Ruleset.Players.StockCount);
                f.Events.OnHideShowReadiness(entity, index, isRealBattle);
            }

            SetPosition(f, entity, FP._1_50);
            f.Events.OnHurtboxStateChange(entity, (HurtboxType)32767, new() { CanBeDamaged = true, CanBeInterrupted = true, CanBeKnockedBack = true, DamageToBreak = 0 });

            return entity;
        }

        public static void DespawnPlayer(Frame f, EntityRef player)
        {
            PlayerStats stats = f.Get<PlayerStats>(player);
            Transform2D transform = f.Get<Transform2D>(player);
            f.Events.OnPlayerDespawn(new() { Entity = player, Index = stats.Index, Name = stats.Name, Position = transform.Position });

            RemovePlayerFromList(f, stats.Index);

            f.Destroy(player);
        }

        public static void SetPosition(Frame f, EntityRef player, FP offset)
        {
            if (!f.Unsafe.TryGetPointer(player, out Transform2D* transform))
                return;

            FighterIndex index = f.Unsafe.GetPointer<PlayerStats>(player)->Index;
            FPVector2 position = Types.ArrayHelper.All(f.Global->CurrentMatch.Stage.Spawn.PlayerSpawnPoints)[index.Global];

            var hit = f.Physics2D.Linecast(position, position + FPVector2.Down * 5, f.RuntimeConfig.GroundLayer);
            if (hit.HasValue)
            {
                Log.Debug("Linecast hit!");
                transform->Position = hit.Value.Point + new FPVector2(0, offset);
            }
            else
            {
                Log.Debug("Linecast failed!");
                transform->Position = position;
            }

            if (f.Unsafe.TryGetPointer(player, out CharacterController* characterController))
            {
                if (index.Global % 2 != 0)
                    characterController->MovementDirection = -1;
                else
                    characterController->MovementDirection = 1;

                f.Events.OnPlayerChangeDirection(player, index, characterController->MovementDirection);
            }
        }

        public static void AddPlayerToList(Frame f, EntityRef player, FighterIndex index)
        {
            if (index.Type == FighterType.Human)
            {
                ++f.Global->TotalPlayers;

                f.Global->PlayerSlotsNoBots[index.GlobalNoBots] = true;
                f.Global->PlayersNoBots[index.GlobalNoBots] = player;
            }
            else
            {
                f.Global->PlayerSlotsNoHumans[index.GlobalNoHumans] = true;
                f.Global->PlayersNoHumans[index.GlobalNoHumans] = player;
            }

            f.Global->PlayerSlots[index.Global] = true;
            f.Global->Players[index.Global] = player;
        }

        public static void RemovePlayerFromList(Frame f, FighterIndex index)
        {
            if (index.Type == FighterType.Human)
            {
                --f.Global->TotalPlayers;

                f.Global->PlayerSlotsNoBots[index.GlobalNoBots] = false;
                f.Global->PlayersNoBots[index.GlobalNoBots] = EntityRef.None;
            }
            else
            {
                f.Global->PlayerSlotsNoHumans[index.GlobalNoHumans] = false;
                f.Global->PlayersNoHumans[index.GlobalNoHumans] = EntityRef.None;
            }

            f.Global->PlayerSlots[index.Global] = false;
            f.Global->Players[index.Global] = EntityRef.None;
        }
    }
}
