using Quantum.Collections;

namespace Quantum
{
    public unsafe class HitboxSystem : SystemMainThreadFilter<HitboxSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public HitboxInstance* HitboxInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            Draw.Circle(filter.Transform->Position + filter.HitboxInstance->Settings.Offset, filter.HitboxInstance->Settings.Radius);

            --filter.HitboxInstance->Lifetime;

            if (filter.HitboxInstance->Lifetime <= 0)
            {
                if (f.Unsafe.TryGetPointer(filter.HitboxInstance->Owner, out Stats* stats))
                {
                    QList<EntityRef> hitboxes = f.ResolveList(stats->Hitboxes);
                    hitboxes.Remove(filter.Entity);
                }

                f.Destroy(filter.Entity);
            }
        }
    }
}