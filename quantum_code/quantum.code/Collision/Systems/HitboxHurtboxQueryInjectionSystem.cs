namespace Quantum
{
    public unsafe class HitboxHurtboxQueryInjectionSystem : SystemMainThread
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public HitboxInstance* HitboxInstance;
        }

        public override void Update(Frame f)
        {
            var hitboxFilter = f.Unsafe.FilterStruct<Filter>();
            var hitbox = default(Filter);

            while (hitboxFilter.Next(&hitbox))
            {
                hitbox.HitboxInstance->PathQueryIndex = f.Physics2D.AddOverlapShapeQuery
                (
                    transform: Transform2D.Create(hitbox.Transform->Position),
                    shape: hitbox.HitboxInstance->Shape,
                    layerMask: f.RuntimeConfig.HitboxLayer
                );
            }
        }
    }
}
