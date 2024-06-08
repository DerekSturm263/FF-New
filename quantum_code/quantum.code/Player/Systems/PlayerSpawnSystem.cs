using Quantum.Collections;

namespace Quantum
{
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            SpawnPlayer(f, player);

            ++f.Global->TotalPlayers;
        }

        public static EntityRef SpawnPlayer(Frame f, PlayerRef player)
        {
            RuntimePlayer data = f.GetPlayerData(player);
            EntityPrototype prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            EntityRef entity = f.Create(prototype);

            PlayerLink playerLink = new()
            {
                Player = player,
                Entity = entity
            };

            Stats* stats = f.Unsafe.GetPointer<Stats>(entity);
            stats->GlobalIndex = player._index - 1;
            StatsSystem.SetBuild(f, entity, stats, stats->Build);

            f.Add(entity, playerLink);
            f.Events.OnPlayerSpawn(entity, stats->GlobalIndex);

            var teams = f.ResolveList(f.Global->CurrentMatch.Teams);

            QListPtr<EntityRef> newTeamPtr = f.AllocateList<EntityRef>();
            var newTeam = f.ResolveList(newTeamPtr);
            newTeam.Add(entity);

            teams.Add(new() { Players = newTeam });

            f.Events.OnPlayerModifyHealth(entity, stats->GlobalIndex, stats->CurrentHealth, stats->CurrentHealth, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);
            f.Events.OnPlayerModifyEnergy(entity, stats->GlobalIndex, stats->CurrentEnergy, stats->CurrentEnergy, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy);
            f.Events.OnPlayerModifyStocks(entity, stats->GlobalIndex, stats->CurrentStocks, stats->CurrentStocks, f.Global->CurrentMatch.Ruleset.Players.StockCount);

            if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
            {
                transform->Position = Types.ArrayHelper.Get(f.Global->CurrentMatch.Stage.Spawn.PlayerSpawnPoints, stats->GlobalIndex);
            }

            return entity;
        }

        public static void DespawnPlayer(Frame f, EntityRef player)
        {
            f.Events.OnPlayerDespawn(player, f.Get<Stats>(player).GlobalIndex);
            f.Destroy(player);

            --f.Global->TotalPlayers;
        }
    }
}
