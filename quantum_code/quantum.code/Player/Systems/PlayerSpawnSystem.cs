using Quantum.Collections;
using System.ComponentModel;

namespace Quantum
{
    public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            RuntimePlayer data = f.GetPlayerData(player);
            EntityPrototype prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            EntityRef entity = f.Create(prototype);

            PlayerLink playerLink = new()
            {
                Player = player,
                Entity = entity
            };

            f.Add(entity, playerLink);
            f.Events.OnPlayerSpawn(playerLink);

            if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
            {
                ++playerCounter->TotalPlayers;
            }

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                StatsSystem.SetBuild(f, entity, stats, stats->Build);
            }

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                var teams = f.ResolveList(matchInstance->Match.Teams);

                QListPtr<PlayerLink> newTeamPtr = f.AllocateList<PlayerLink>();
                var newTeam = f.ResolveList(newTeamPtr);
                newTeam.Add(playerLink);

                teams.Add(new() { Players = newTeam });

                if (f.Unsafe.TryGetPointer(entity, out Stats* stats2))
                {
                    f.Events.OnPlayerModifyHealth(playerLink, stats2->CurrentHealth, stats2->CurrentHealth, matchInstance->Match.Ruleset.Players.MaxHealth);
                    f.Events.OnPlayerModifyEnergy(playerLink, stats2->CurrentEnergy, stats2->CurrentEnergy, matchInstance->Match.Ruleset.Players.MaxEnergy);
                    f.Events.OnPlayerModifyStocks(playerLink, stats2->CurrentStocks, stats2->CurrentStocks, matchInstance->Match.Ruleset.Players.StockCount);
                }

                if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
                {
                    transform->Position = Types.ArrayHelper.Get(matchInstance->Match.Stage.Spawn.PlayerSpawnPoints, player._index - 1);
                }
            }
        }
    }
}
