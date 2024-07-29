﻿using Photon.Deterministic;
using Quantum.Types;

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
            itemAsset = ArrayHelper.Get(f.Global->CurrentMatch.Ruleset.Items.Items, f.Global->RngSession.Next(0, 16));
            
            FPVector2 spawnPosition = ArrayHelper.Get(f.Global->CurrentMatch.Stage.Spawn.ItemSpawnPoints, f.Global->RngSession.Next(0, 16));
            ItemSpawnSettings settings = new()
            {
                Item = itemAsset,
                Velocity = FPVector2.Zero,
                Offset = spawnPosition + new FPVector2(0, 4),
                StartHolding = false
            };

            return Spawn(f, settings);
        }

        public static EntityRef Spawn(Frame f, ItemSpawnSettings settings)
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

            return newItem;
        }

        public static EntityRef SpawnWithOwner(Frame f, ItemSpawnSettings settings, EntityRef owner)
        {
            EntityRef item = Spawn(f, settings);

            if (f.Unsafe.TryGetPointer(item, out ItemInstance* itemInstance))
                itemInstance->Owner = owner;

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