namespace Quantum
{
    public unsafe class HitboxHurtboxQueryInjectionSystem : SystemMainThread
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform3D* Transform;
            public HitboxInstance* HitboxInstance;
        }

        public override void Update(Frame f)
        {
            var hitboxFilter = f.Unsafe.FilterStruct<Filter>();
            var hitbox = default(Filter);

            while (hitboxFilter.Next(&hitbox))
            {
                Shape2D shape2D = new()
                {
                    BroadRadius = hitbox.HitboxInstance->Shape.BroadRadius,
                    Centroid = hitbox.HitboxInstance->Shape.Centroid.XY,
                    Box = new()
                    {
                        Extents = hitbox.HitboxInstance->Shape.Box.Extents.XY
                    }
                };

                hitbox.HitboxInstance->PathQueryIndex = f.Physics2D.AddOverlapShapeQuery
                (
                    transform: Transform2D.Create(hitbox.Transform->Position.XY, hitbox.Transform->Rotation.Z),
                    shape: shape2D,
                    layerMask: f.RuntimeConfig.HitboxLayer
                );
            }
        }
    }
}
