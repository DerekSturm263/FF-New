using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class MagneticBadge : Badge
    {
        public FP Speed;

        public FP FollowRadius;
        public FP GrabRadius;

        public override void OnUpdate(Frame f, ref CharacterControllerSystem.Filter filter)
        {
            var itemFilter = f.Unsafe.FilterStruct<ItemSystem.Filter>();
            var item = default(ItemSystem.Filter);

            while (itemFilter.Next(&item))
            {
                if (item.ItemInstance->IsActive)
                    continue;

                FP distance = FPVector2.DistanceSquared(filter.Transform->Position, item.Transform->Position);

                if (distance <= FollowRadius * FollowRadius)
                {
                    item.PhysicsBody->Velocity = (filter.Transform->Position - item.Transform->Position).Normalized * Speed;

                    if (distance <= GrabRadius * GrabRadius)
                    {
                        if (!filter.PlayerStats->HeldItem.IsValid)
                            ItemSystem.Use(f, ref filter, item.Entity);
                    }
                }
            }
        }
    }
}
