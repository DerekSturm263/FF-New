using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class MagneticBadge : Badge
    {
        public FP Speed;

        public FP FollowRadius;
        public FP GrabRadius;

        public override void OnUpdate(Frame f, EntityRef user)
        {
            var itemFilter = f.Unsafe.FilterStruct<ItemSystem.Filter>();
            var item = default(ItemSystem.Filter);

            if (!f.TryGet(user, out Transform2D transform))
                return;

            while (itemFilter.Next(&item))
            {
                if (!f.Unsafe.TryGetPointer(item.Entity, out ItemInstance* itemInstance))
                    continue;

                if (itemInstance->IsActive)
                    continue;

                FP distance = FPVector2.DistanceSquared(transform.Position, item.Transform->Position);

                if (distance <= FollowRadius * FollowRadius)
                {
                    item.PhysicsBody->Velocity = (transform.Position - item.Transform->Position).Normalized * Speed;

                    if (distance <= GrabRadius * GrabRadius)
                    {
                        if (f.Unsafe.TryGetPointer(user, out PlayerStats* stats) && !stats->HeldItem.IsValid)
                            ItemSystem.Use(f, user, item.Entity, itemInstance);
                    }
                }
            }
        }
    }
}
