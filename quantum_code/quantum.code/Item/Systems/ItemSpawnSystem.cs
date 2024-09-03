using Photon.Deterministic;
using Quantum.Types;
using System.Collections.Generic;

namespace Quantum
{
    public unsafe class ItemSpawnSystem : SystemMainThreadFilter<ItemSpawnSystem.Filter>
    {
        public override bool StartEnabled => false;

        public struct Filter
        {
            public EntityRef Entity;
            public ItemSpawner* ItemSpawner;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (f.Global->CurrentMatch.Ruleset.Items.SpawnFrequency == 0)
                return;

            if (filter.ItemSpawner->TimeSinceLastSpawned <= 0)
            {
                if (filter.ItemSpawner->CurrentSpawned == filter.ItemSpawner->MaxAllowed)
                    return;

                SpawnRandom(f);

                filter.ItemSpawner->TimeSinceLastSpawned = f.Global->RngSession.Next(filter.ItemSpawner->MinTimeToSpawn, filter.ItemSpawner->MaxTimeToSpawn) * (1 / f.Global->CurrentMatch.Ruleset.Items.SpawnFrequency);
            }
            else
            {
                filter.ItemSpawner->TimeSinceLastSpawned -= f.DeltaTime;
            }
        }
        
        public static EntityRef SpawnRandom(Frame f)
        {
            AssetRefItem itemAsset = default;

            List<AssetRefItem> allItems = ArrayHelper.All(f.Global->CurrentMatch.Ruleset.Items.Items);
            if (allItems.Count == 0)
                return EntityRef.None;
            
            itemAsset = allItems[f.Global->RngSession.Next(0, allItems.Count)];

            FP x = f.Global->RngSession.Next(f.Global->CurrentMatch.Stage.Spawn.MinPoint.X, f.Global->CurrentMatch.Stage.Spawn.MaxPoint.X);
            FP y = f.Global->RngSession.Next(f.Global->CurrentMatch.Stage.Spawn.MinPoint.Y, f.Global->CurrentMatch.Stage.Spawn.MaxPoint.Y);
            FPVector2 spawnPosition = new(x, y);

            var hit = f.Physics2D.Linecast(spawnPosition, spawnPosition + FPVector2.Down * 10, f.RuntimeConfig.GroundLayer);
            if (hit.HasValue)
            {
                ItemSpawnSettings settings = new()
                {
                    Item = itemAsset,
                    Velocity = FPVector2.Zero,
                    Offset = hit.Value.Point + new FPVector2(0, 4),
                    StartHolding = false
                };

                return Spawn(f, settings);
            }

            return EntityRef.None;
        }

        public static EntityRef Spawn(Frame f, ItemSpawnSettings settings, EntityRef owner = default)
        {
            EntityRef newItem = default;

            Item item = f.FindAsset<Item>(settings.Item.Id);
            newItem = f.Create(item.Prototype);

            if (f.Unsafe.TryGetPointer(newItem, out Transform2D* transform))
            {
                transform->Position = settings.Offset;
            }

            if (f.Unsafe.TryGetPointer(newItem, out ItemInstance* itemInstance))
            {
                itemInstance->FallState = settings.Velocity == FPVector2.Zero;
                itemInstance->FallY = settings.Offset.Y;
                itemInstance->Item = item;
                itemInstance->Owner = owner;
                itemInstance->MaxCollisions = 1;
            }

            if (f.Unsafe.TryGetPointer(newItem, out PhysicsBody2D* physicsBody))
            {
                if (settings.Velocity != FPVector2.Zero)
                {
                    physicsBody->Velocity = settings.Velocity;
                }
                else
                {
                    physicsBody->Enabled = false;
                }
            }

            if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                ++itemSpawner->CurrentSpawned;

            if (item is UpdateableItem updateableItem)
                updateableItem.OnStart(f, owner, newItem, itemInstance);

            return newItem;
        }

        public static EntityRef SpawnParented(Frame f, ItemSpawnSettings settings, EntityRef owner)
        {
            EntityRef item = Spawn(f, settings, owner);

            if (f.Unsafe.TryGetPointer(item, out Transform2D* transform) &&
                f.Unsafe.TryGetPointer(owner, out Transform2D* parentTransform))
            {
                transform->Position += parentTransform->Position;
            }

            if (settings.StartHolding)
                ItemSystem.PickUp(f, owner, item);

            return item;
        }

        public static void Despawn(Frame f, EntityRef item)
        {
            f.Destroy(item);

            if (f.Unsafe.TryGetPointerSingleton(out ItemSpawner* itemSpawner))
                --itemSpawner->CurrentSpawned;
        }

        public static void DespawnAll(Frame f)
        {
            foreach (var item in f.GetComponentIterator<ItemInstance>())
            {
                Despawn(f, item.Entity);
            }
        }
    }
}