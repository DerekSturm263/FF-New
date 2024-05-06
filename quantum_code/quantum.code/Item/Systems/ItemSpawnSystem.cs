using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ItemSpawnSystem : SystemMainThreadFilter<ItemSpawnSystem.Filter>, ISignalOnComponentAdded<ItemSpawner>
    {
        public override bool StartEnabled => false;

        public struct Filter
        {
            public EntityRef Entity;
            public ItemSpawner* ItemSpawner;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.ItemSpawner->TimeSinceLastSpawned <= 0)
            {
                if (filter.ItemSpawner->CurrentSpawned == filter.ItemSpawner->MaxAllowed)
                    return;

                SpawnItemRandom(f);

                MatchInstance* matchInstance = f.Unsafe.GetPointerSingleton<MatchInstance>();
                filter.ItemSpawner->TimeSinceLastSpawned = f.Global->RngSession.Next(filter.ItemSpawner->MinTimeToSpawn, filter.ItemSpawner->MaxTimeToSpawn) * (1 / matchInstance->Match.Ruleset.Items.SpawnFrequency);
            }
            else
            {
                filter.ItemSpawner->TimeSinceLastSpawned -= f.DeltaTime;
            }
        }

        public void OnAdded(Frame f, EntityRef entity, ItemSpawner* component)
        {
            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                component->TimeSinceLastSpawned = f.Global->RngSession.Next(component->MinTimeToSpawn, component->MaxTimeToSpawn) * (1 / matchInstance->Match.Ruleset.Items.SpawnFrequency);
            }
        }

        public static EntityRef SpawnItemRandom(Frame f)
        {
            MatchInstance* matchInstance = f.Unsafe.GetPointerSingleton<MatchInstance>();
            
            AssetRefItem itemAsset = default;
            itemAsset = matchInstance->Match.Ruleset.Items.Items[f.Global->RngSession.Next(0, matchInstance->Match.Ruleset.Items.Items.Length)];
            
            FPVector2 spawnPosition = matchInstance->Match.Stage.Spawn.ItemSpawnPoints[f.Global->RngSession.Next(0, matchInstance->Match.Stage.Spawn.ItemSpawnPoints.Length)];
            
            return SpawnItem(f, itemAsset, spawnPosition);
        }

        public static EntityRef SpawnItem(Frame f, AssetRefItem toSpawn, FPVector2 position)
        {
            EntityRef newItem = default;

            if (f.Unsafe.TryGetPointerSingleton(out MatchInstance* matchInstance))
            {
                Item item = f.FindAsset<Item>(toSpawn.Id);
                newItem = f.Create(item.Prototype);

                if (f.Unsafe.TryGetPointer(newItem, out Transform2D* transform))
                {
                    transform->Position = position;
                }
            }

            if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                ++itemSpawner->CurrentSpawned;

            return newItem;
        }
    }
}