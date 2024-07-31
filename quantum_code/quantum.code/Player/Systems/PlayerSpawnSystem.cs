using Quantum.Collections;

namespace Quantum
{
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            RuntimePlayer data = f.GetPlayerData(player);
            EntityPrototype prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

            SpawnPlayer(f, player, prototype, true, data.Name, data.Index, data.IsRealBattle);
        }

        public static EntityRef SpawnPlayer(Frame f, PlayerRef player, AssetRefEntityPrototype prototype, bool assignLink, string name, FighterIndex index, bool isRealBattle)
        {
            EntityRef entity = f.Create(prototype);

            PlayerLink playerLink = new()
            {
                Player = player,
                Entity = entity
            };

            PlayerStats* playerStats = f.Unsafe.GetPointer<PlayerStats>(entity);
            playerStats->Index = index;

            PlayerStatsSystem.SetBuild(f, entity, playerStats, playerStats->Build);
            AddPlayerToList(f, entity, index);

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

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                f.Events.OnPlayerModifyHealth(entity, index, stats->CurrentStats.Health, stats->CurrentStats.Health, f.Global->CurrentMatch.Ruleset.Players.MaxHealth);
                f.Events.OnPlayerModifyEnergy(entity, index, stats->CurrentStats.Energy, stats->CurrentStats.Energy, f.Global->CurrentMatch.Ruleset.Players.MaxEnergy);
                f.Events.OnPlayerModifyStocks(entity, index, stats->CurrentStats.Stocks, stats->CurrentStats.Stocks, f.Global->CurrentMatch.Ruleset.Players.StockCount);
                f.Events.OnHideShowReadiness(entity, index, isRealBattle);
            }

            if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
            {
                transform->Position = Types.ArrayHelper.Get(f.Global->CurrentMatch.Stage.Spawn.PlayerSpawnPoints, index.Global);
            }

            return entity;
        }

        public static void DespawnPlayer(Frame f, EntityRef player)
        {
            PlayerStats stats = f.Get<PlayerStats>(player);
            f.Events.OnPlayerDespawn(player, stats.Index, stats.Name);

            RemovePlayerFromList(f, stats.Index);
            f.Destroy(player);
        }

        public static void AddPlayerToList(Frame f, EntityRef player, FighterIndex index)
        {
            if (index.Type == FighterType.Human)
            {
                ++f.Global->TotalPlayers;

                f.Global->PlayerSlotsNoBots[index.GlobalNoBots] = true;
                f.Global->PlayersNoBots[index.GlobalNoBots] = player;
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

            f.Global->PlayerSlots[index.Global] = false;
            f.Global->Players[index.Global] = EntityRef.None;
        }
    }
}
