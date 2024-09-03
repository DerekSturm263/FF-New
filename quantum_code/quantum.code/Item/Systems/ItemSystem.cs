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
                    updateableItem.OnUpdate(f, filter.ItemInstance->Owner, filter.Entity, filter.ItemInstance);
            }

            if (!filter.ItemInstance->Holder.IsValid)
                filter.ItemInstance->ActiveTime += f.DeltaTime;
        }

        public void OnRemoved(Frame f, EntityRef entity, ItemInstance* component)
        {
            if (f.TryFindAsset(component->Item.Id, out Item item))
            {
                if (item is UpdateableItem updateableItem)
                    updateableItem.OnExit(f, component->Owner, entity, component);
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

                    if (itemInstance->IsActive && (itemInstance->Collisions >= itemInstance->MaxCollisions || f.Unsafe.TryGetPointer(info.Other, out CharacterController* _)))
                    {
                        item.OnHit(f, itemInstance->Owner, info.Other, info.Entity, itemInstance);
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
                    if (f.TryFindAsset(stats->Build.Gear.SubWeapon.Enhancer.Id, out SubEnhancer enhancer))
                        enhancer.OnHit(f, itemInstance->Owner, info.Other, info.Entity, stats->Build.Gear.SubWeapon);

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

        public static void Use(Frame f, EntityRef user, EntityRef itemEntity, ItemInstance* itemInstance)
        {
            Item item = f.FindAsset<Item>(itemInstance->Item.Id);
            item.Invoke(f, user, itemEntity, itemInstance);

            itemInstance->FallState = false;
            itemInstance->IsActive = true;

            if (f.Unsafe.TryGetPointer(user, out PlayerStats* stats))
                ++stats->Stats.ItemUses;
        }
    }
}
