using Quantum.Collections;

namespace Quantum
{
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            RuntimePlayer data = f.GetPlayerData(player);
            EntityPrototype prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            SpawnPlayer(f, player, prototype, true, data.DeviceIndex, data.Name);

            ++f.Global->TotalPlayers;
        }

        public static EntityRef SpawnPlayer(Frame f, PlayerRef player, AssetRefEntityPrototype prototype, bool assignLink, int deviceIndex, string name)
        {
            EntityRef entity = f.Create(prototype);

            PlayerLink playerLink = new()
            {
                Player = player,
                Entity = entity
            };

            Stats* stats = f.Unsafe.GetPointer<Stats>(entity);
            StatsSystem.SetBuild(f, entity, stats, stats->Build);

            AddPlayerToList(f, entity, deviceIndex);
            stats->DeviceIndex = deviceIndex;

            FighterIndex index = stats->GetIndex(f, entity);

            if (assignLink)
            {
                f.Add(entity, playerLink);
            }

            f.Events.OnPlayerSpawn(entity, index, name);

            var teams = f.ResolveList(f.Global->Teams);

            QListPtr<EntityRef> newTeamPtr = f.AllocateList<EntityRef>();
            var newTeam = f.ResolveList(newTeamPtr);
            newTeam.Add(entity);

            teams.Add(new() { Players = newTeam });

            f.Events.OnPlayerModifyHealth(entity, index, stats->CurrentHealth, stats->CurrentHealth, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);
            f.Events.OnPlayerModifyEnergy(entity, index, stats->CurrentEnergy, stats->CurrentEnergy, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy);
            f.Events.OnPlayerModifyStocks(entity, index, stats->CurrentStocks, stats->CurrentStocks, f.Global->CurrentMatch.Ruleset.Players.StockCount);

            if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
            {
                transform->Position = Types.ArrayHelper.Get(f.Global->CurrentMatch.Stage.Spawn.PlayerSpawnPoints, index.Global);
            }

            return entity;
        }

        public static void DespawnPlayer(Frame f, EntityRef player)
        {
            Stats stats = f.Get<Stats>(player);
            RemovePlayerFromList(f, player, stats);

            f.Events.OnPlayerDespawn(player, stats.GetIndex(f, player), stats.Name);
            f.Destroy(player);

            --f.Global->TotalPlayers;
        }

        public static bool AddPlayerToList(Frame f, EntityRef player, int deviceIndex)
        {
            for (int i = 0; i < 4; ++i)
            {
                if (!f.Global->AllPlayers[deviceIndex * 4 + i].IsValid)
                {
                    f.Global->AllPlayers[deviceIndex * 4 + i] = player;
                    return true;
                }
            }

            return false;
        }

        public static void RemovePlayerFromList(Frame f, EntityRef player, Stats stats)
        {
            FighterIndex index = stats.GetIndex(f, player);
            f.Global->AllPlayers[index.Internal] = EntityRef.None;
        }
    }
}
