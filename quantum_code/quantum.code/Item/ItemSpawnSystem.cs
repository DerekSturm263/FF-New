using Photon.Deterministic;
using Quantum.Collections;

namespace Quantum
{
    public unsafe class ItemSpawnSystem : SystemMainThreadFilter<ItemSpawnSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public ItemSpawner* Spawner;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.Spawner->TimeSinceLastSpawned <= 0)
            {
                EntityRef newItem = f.Create(filter.Spawner->Prototype);
                
                if (f.Unsafe.TryGetPointer(newItem, out Transform2D* transform))
                {
                    if (f.Unsafe.TryGetPointerSingleton(out StageInstance* stageInstance))
                    {
                        var spawnPoints = f.ResolveList(stageInstance->Stage.Spawn.ItemSpawnPoints);
                        transform->Position = spawnPoints[f.Global->RngSession.Next(0, spawnPoints.Count)];
                    }
                    else
                    {
                        transform->Position = new(0, 10);
                    }
                }

                filter.Spawner->TimeSinceLastSpawned = f.Global->RngSession.Next(filter.Spawner->MinTimeToSpawn, filter.Spawner->MaxTimeToSpawn);

                if (f.Unsafe.TryGetPointerSingleton(out RulesetInstance* rulesetInstance))
                {
                    filter.Spawner->TimeSinceLastSpawned *= 1 / rulesetInstance->Ruleset.Items.SpawnFrequency;
                }
            }
            else
            {
                filter.Spawner->TimeSinceLastSpawned -= f.DeltaTime;
            }
        }
    }
}