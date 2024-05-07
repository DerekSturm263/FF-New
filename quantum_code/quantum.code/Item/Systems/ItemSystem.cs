using Photon.Deterministic;

namespace Quantum
{
    public unsafe class ItemSystem : SystemMainThreadFilter<ItemSystem.Filter>, ISignalOnCollisionEnter2D
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

                if (filter.Transform->Position.Y - filter.ItemInstance->FallY < FP._0_01)
                {
                    filter.ItemInstance->FallState = false;
                    filter.PhysicsBody->Enabled = true;
                }
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

                if (info.Other == itemInstance->Owner)
                    return;

                if (f.TryFindAsset(itemInstance->Item.Id, out Item item))
                {
                    if (item is HoldableItem holdableItem)
                    {
                        if (f.Unsafe.TryGetPointer(itemInstance->Owner, out PlayerLink* playerLink))
                        {
                            holdableItem.OnHit(f, playerLink, info.Other, info.Entity, itemInstance);
                        }
                    }
                }
            }
        }

        public static void PickUp(Frame f, EntityRef user, EntityRef item)
        {
            if (f.Unsafe.TryGetPointer(item, out ItemInstance* itemInstance))
            {
                if (f.Unsafe.TryGetPointer(user, out Stats* stats))
                    stats->HeldItem = item;

                itemInstance->Holder = user;
                itemInstance->Owner = user;

                if (f.Unsafe.TryGetPointer(item, out PhysicsCollider2D* physicsCollider))
                    physicsCollider->Enabled = false;

                if (f.Unsafe.TryGetPointer(item, out PhysicsBody2D* physicsBody))
                    physicsBody->GravityScale = 1;

                itemInstance->FallState = false;
            }
        }

        public static void Throw(Frame f, EntityRef user, EntityRef item, FPVector2 force)
        {
            if (f.Unsafe.TryGetPointer(item, out ItemInstance* itemInstance))
            {
                if (f.Unsafe.TryGetPointer(user, out Stats* stats))
                    stats->HeldItem = default;

                itemInstance->Holder = default;

                if (f.Unsafe.TryGetPointer(item, out PhysicsBody2D* physicsBody))
                    physicsBody->Velocity = force;

                if (f.Unsafe.TryGetPointer(item, out PhysicsCollider2D* physicsCollider))
                    physicsCollider->Enabled = true;

                if (f.Unsafe.TryGetPointer(item, out Transform2D* transform))
                    transform->Position += new FPVector2(2, 2);
            }
        }

        public static void Use(Frame f, PlayerLink* user, EntityRef itemEntity, ItemInstance* itemInstance)
        {
            Item item = f.FindAsset<Item>(itemInstance->Item.Id);
            item.Invoke(f, user, itemEntity, itemInstance);

            itemInstance->FallState = false;
        }
    }
}
