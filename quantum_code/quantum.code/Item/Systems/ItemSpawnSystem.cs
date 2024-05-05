using Photon.Deterministic;
using Quantum.Collections;
using System.IO.MemoryMappedFiles;

namespace Quantum
{
    public unsafe class ItemSpawnSystem : SystemMainThreadFilter<ItemSpawnSystem.Filter>
    {
        public override bool StartEnabled => false;

        public struct Filter
        {
            public EntityRef Entity;
            public ItemSpawner* Spawner;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.Spawner->TimeSinceLastSpawned <= 0)
            {
                if (filter.Spawner->CurrentSpawned == filter.Spawner->MaxAllowed)
                    return;

                AssetRefItem itemAsset = default;
                EntityRef newItem = default;

                if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
                {
                    itemAsset = matchInstance->Match.Ruleset.Items.Items[f.Global->RngSession.Next(0, matchInstance->Match.Ruleset.Items.Items.Length)];

                    Item item = f.FindAsset<Item>(itemAsset.Id);
                    newItem = f.Create(item.Prototype);

                    if (f.Unsafe.TryGetPointer(newItem, out Transform2D* transform))
                    {
                        transform->Position = matchInstance->Match.Stage.Spawn.ItemSpawnPoints[f.Global->RngSession.Next(0, matchInstance->Match.Stage.Spawn.ItemSpawnPoints.Length)];
                    }

                    filter.Spawner->TimeSinceLastSpawned = f.Global->RngSession.Next(filter.Spawner->MinTimeToSpawn, filter.Spawner->MaxTimeToSpawn) * (1 / matchInstance->Match.Ruleset.Items.SpawnFrequency);
                }

                ++filter.Spawner->CurrentSpawned;
            }
            else
            {
                filter.Spawner->TimeSinceLastSpawned -= f.DeltaTime;
            }
        }
    }
}