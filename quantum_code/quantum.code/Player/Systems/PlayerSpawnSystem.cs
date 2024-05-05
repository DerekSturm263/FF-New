using Quantum.Collections;

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

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                var teams = f.ResolveList(matchInstance->Match.Teams);

                QListPtr<PlayerLink> newTeamPtr = f.AllocateList<PlayerLink>();
                var newTeam = f.ResolveList(newTeamPtr);
                newTeam.Add(playerLink);

                teams.Add(new() { Players = newTeam });

                if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
                {
                    f.Events.OnPlayerModifyHealth(playerLink, stats->CurrentHealth, stats->CurrentHealth, stats->MaxHealth);
                    f.Events.OnPlayerModifyEnergy(playerLink, stats->CurrentEnergy, stats->CurrentEnergy, stats->MaxEnergy);
                    f.Events.OnPlayerModifyStocks(playerLink, stats->CurrentStocks, stats->CurrentStocks, stats->MaxStocks);
                }

                if (f.Unsafe.TryGetPointer(entity, out Transform2D* transform))
                {
                    transform->Position = matchInstance->Match.Stage.Spawn.PlayerSpawnPoints[player._index - 1];
                }
            }
        }
    }
}
