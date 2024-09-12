using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ItemSystem : SystemMainThreadFilter<ItemSystem.Filter>, ISignalOnCollisionEnter2D, ISignalOnComponentRemoved<ItemInstance>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public PhysicsBody2D* PhysicsBody;
            public ItemInstance* ItemInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (filter.ItemInstance->Holder.IsValid)
            {
                HoldInHand(f, filter.ItemInstance->Holder, filter.Transform);
            }
            else if (filter.ItemInstance->FallState)
            {
                filter.Transform->Position.Y = FPMath.Lerp(filter.Transform->Position.Y, filter.ItemInstance->FallY, f.DeltaTime * filter.ItemInstance->FallSpeed);

                if (filter.Transform->Position.Y - filter.ItemInstance->FallY < FP._0_05)
                {
                    filter.ItemInstance->FallState = false;
                    filter.PhysicsBody->Enabled = true;
                }
            }

            if (f.TryFindAsset(filter.ItemInstance->Item.Id, out Item item))
            {
                if (item is UpdateableItem updateableItem)
                    updateableItem.OnUpdate(f, filter.ItemInstance->Owner, ref filter);

                if (item.AlignDirectionToVelocity)
                    filter.Transform->Rotation = FPMath.Atan2(filter.PhysicsBody->Velocity.Y, filter.PhysicsBody->Velocity.X);
            }

            if (!filter.ItemInstance->Holder.IsValid)
                ++filter.ItemInstance->ActiveTime;
        }

        public void OnRemoved(Frame f, EntityRef entity, ItemInstance* component)
        {
            if (f.TryFindAsset(component->Item.Id, out Item item) && f.Unsafe.ComponentGetter<Filter>().TryGet(f, entity, out Filter filter))
            {
                if (item is UpdateableItem updateableItem)
                    updateableItem.OnExit(f, component->Owner, ref filter);
            }
        }

        private void HoldInHand(Frame f, EntityRef holder, Transform2D* transform)
        {
            if (f.Unsafe.TryGetPointer(holder, out Stats* stats))
            {
                var dict = f.ResolveDictionary(stats->Hurtboxes);

                if (f.Unsafe.TryGetPointer(dict[HurtboxType.RightHand], out Transform2D* handTransform))
                {
                    transform->Position = handTransform->Position;
                }
            }
        }

        public void OnCollisionEnter2D(Frame f, CollisionInfo2D info)
        {
            if (f.Unsafe.TryGetPointer(info.Entity, out ItemInstance* itemInstance))
            {
                if (itemInstance->Holder.IsValid)
                    return;

                ++itemInstance->Collisions;

                if (f.TryFindAsset(itemInstance->Item.Id, out Item item))
                {
                    if (!item.CanInteractWithOwner && info.Other == itemInstance->Owner)
                        return;

                    bool isPlayer = f.Unsafe.TryGetPointer(info.Other, out CharacterController* _);

                    bool overrideCollisionCount = (item.DestroyOnAnyHit.HasFlag(ItemCollisionType.Player) && isPlayer) ||
                                                  (item.DestroyOnAnyHit.HasFlag(ItemCollisionType.Environment) && !isPlayer);

                    if (itemInstance->IsActive && (itemInstance->Collisions >= itemInstance->MaxCollisions || overrideCollisionCount))
                    {
                        bool canHit = (item.DestroyOnHitAfterMinCollisions.HasFlag(ItemCollisionType.Player) && isPlayer) ||
                                      (item.DestroyOnHitAfterMinCollisions.HasFlag(ItemCollisionType.Environment) && !isPlayer);

                        if (canHit && f.Unsafe.ComponentGetter<Filter>().TryGet(f, info.Entity, out Filter filter) && item.OnHit(f, itemInstance->Owner, info.Other, ref filter))
                            ItemSpawnSystem.Despawn(f, info.Entity);
                    }
                }

                if (itemInstance->DoBounce && f.Unsafe.TryGetPointer(info.Entity, out PhysicsBody2D* physicsBody))
                {
                    if (FPVector2.Dot(info.ContactNormal, FPVector2.Up) > FP._0_50)
                        physicsBody->Velocity.Y = info.ContactNormal.Y * 10;
                    else
                        physicsBody->Velocity.X = info.ContactNormal.X * 10;
                }
            }

            if (f.Unsafe.TryGetPointer(itemInstance->Owner, out PlayerStats* stats))
            {
                if (f.TryFindAsset(stats->Build.Gear.SubWeapon.Template.Id, out SubTemplate subWeaponTemplate))
                {
                    if (f.TryFindAsset(stats->Build.Gear.SubWeapon.Enhancer.Id, out SubEnhancer enhancer) && f.Unsafe.ComponentGetter<CharacterControllerSystem.Filter>().TryGet(f, itemInstance->Owner, out CharacterControllerSystem.Filter filter))
                        enhancer.OnHit(f, ref filter, info.Other, info.Entity, stats->Build.Gear.SubWeapon);

                    subWeaponTemplate.OnHit(f, itemInstance->Owner, info.Other, info.Entity, stats->Build.Gear.SubWeapon);
                }
            }
        }

        public static void PickUp(Frame f, EntityRef user, EntityRef item)
        {
            if (f.Unsafe.TryGetPointer(item, out ItemInstance* itemInstance))
            {
                if (f.Unsafe.TryGetPointer(user, out PlayerStats* stats))
                    stats->HeldItem = item;

                itemInstance->Holder = user;
                itemInstance->Owner = user;

                if (f.Unsafe.TryGetPointer(item, out PhysicsCollider2D* physicsCollider))
                {
                    physicsCollider->Enabled = false;
                    physicsCollider->Layer = f.RuntimeConfig.ItemLayer;
                }

                if (f.Unsafe.TryGetPointer(item, out PhysicsBody2D* physicsBody))
                {
                    physicsBody->Enabled = true;

                    Item itemAsset = f.FindAsset<Item>(itemInstance->Item.Id);
                    if (itemAsset is not SeekingItem seekingItem)
                        physicsBody->GravityScale = 1;
                }

                itemInstance->FallState = false;
            }
        }

        public static void Throw(Frame f, EntityRef user, EntityRef item, FPVector2 offset, FPVector2 force)
        {
            if (f.Unsafe.TryGetPointer(item, out ItemInstance* itemInstance))
            {
                if (f.Unsafe.TryGetPointer(user, out PlayerStats* stats))
                    stats->HeldItem = default;

                itemInstance->Holder = default;

                if (f.Unsafe.TryGetPointer(item, out PhysicsBody2D* physicsBody))
                    physicsBody->Velocity = force;

                if (f.Unsafe.TryGetPointer(item, out PhysicsCollider2D* physicsCollider))
                    physicsCollider->Enabled = true;

                if (f.Unsafe.TryGetPointer(item, out Transform2D* transform))
                    transform->Position += offset;

                itemInstance->IsActive = true;
            }
        }

        public static void Use(Frame f, EntityRef user, EntityRef itemEntity)
        {
            if (f.Unsafe.ComponentGetter<Filter>().TryGet(f, itemEntity, out Filter filter))
            {
                Item item = f.FindAsset<Item>(filter.ItemInstance->Item.Id);
                item.Invoke(f, user, ref filter);

                filter.ItemInstance->FallState = false;
                filter.ItemInstance->IsActive = true;

                if (f.Unsafe.TryGetPointer(user, out PlayerStats* stats))
                    ++stats->Stats.ItemUses;
            }
        }
    }
}
