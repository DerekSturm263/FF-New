using Quantum.Collections;

namespace Quantum
{
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            SpawnPlayer(f, player);
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

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                stats->PlayerIndex = player._index - 1;
                StatsSystem.SetBuild(f, entity, stats, stats->Build);
            }

            f.Add(entity, playerLink);
            f.Events.OnPlayerSpawn(entity, stats->PlayerIndex);

            if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
                ++playerCounter->TotalPlayers;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                var teams = f.ResolveList(matchInstance->Match.Teams);

                QListPtr<EntityRef> newTeamPtr = f.AllocateList<EntityRef>();
                var newTeam = f.ResolveList(newTeamPtr);
                newTeam.Add(entity);

                teams.Add(new() { Players = newTeam });

                if (f.Unsafe.TryGetPointer(entity, out Stats* stats2))
                {
                    f.Events.OnPlayerModifyHealth(entity, stats->PlayerIndex, stats2->CurrentHealth, stats2->CurrentHealth, matchInstance->Match.Ruleset.Players.MaxHealth);
                    f.Events.OnPlayerModifyEnergy(entity, stats->PlayerIndex, stats2->CurrentEnergy, stats2->CurrentEnergy, matchInstance->Match.Ruleset.Players.MaxEnergy);
                    f.Events.OnPlayerModifyStocks(entity, stats->PlayerIndex, stats2->CurrentStocks, stats2->CurrentStocks, matchInstance->Match.Ruleset.Players.StockCount);
                }

                if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
                {
                    transform->Position = Types.ArrayHelper.Get(matchInstance->Match.Stage.Spawn.PlayerSpawnPoints, player._index - 1);
                }
            }

            return entity;
        }

        public static void DespawnPlayer(Frame f, EntityRef player)
        {
            f.Destroy(player);
            f.Events.OnPlayerDespawn(player, f.Get<Stats>(player).PlayerIndex);
        }
    }
}
