using Photon.Deterministic;

namespace Quantum
{
    public unsafe class HitboxHurtboxQueryInjectionSystem : SystemMainThread
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public HitboxInstance* Hitbox;
        }

        public override void Update(Frame f)
        {
            var hitboxFilter = f.Unsafe.FilterStruct<Filter>();
            var hitbox = default(Filter);

            while (hitboxFilter.Next(&hitbox))
            {
                Shape2D shape2D = Shape2D.CreateCircle(hitbox.Hitbox->Settings.Radius, hitbox.Hitbox->Settings.Offset);
                FPVector2 offset = default;

                if (f.Unsafe.TryGetPointer(hitbox.Hitbox->Parent, out Transform2D* transform))
                    offset += transform->Position;

                hitbox.Hitbox->PathQueryIndex = f.Physics2D.AddOverlapShapeQuery
                (
                    position: hitbox.Transform->Position + offset,
                    rotation: 0,
                    shape: shape2D,
                    layerMask: f.RuntimeConfig.HitboxLayer
                );
            }
        }
    }
}
