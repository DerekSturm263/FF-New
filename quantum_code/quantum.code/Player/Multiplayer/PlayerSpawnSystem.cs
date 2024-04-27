using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Multiplayer
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

            if (f.Unsafe.TryGetPointer(entity, out Transform3D* transform3D))
            {
                transform3D->Position.X = (int)player;
            }

            if (f.Unsafe.TryGetPointer(entity, out Stats* stats))
            {
                f.Events.OnPlayerModifyHealth(playerLink, stats->CurrentHealth, stats->CurrentHealth, stats->MaxHealth);
                f.Events.OnPlayerModifyEnergy(playerLink, stats->CurrentEnergy, stats->CurrentEnergy, stats->MaxEnergy);
                f.Events.OnPlayerModifyStocks(playerLink, stats->CurrentStocks, stats->CurrentStocks, stats->MaxStocks);
            }

            if (f.Unsafe.TryGetPointerSingleton(out PlayerCounter* playerCounter))
            {
                ++playerCounter->TotalPlayers;
            }
        }
    }
}
