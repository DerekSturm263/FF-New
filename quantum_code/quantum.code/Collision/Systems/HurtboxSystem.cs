using Photon.Deterministic;

namespace Quantum
{
    public unsafe class HurtboxSystem : SystemMainThreadFilter<HurtboxSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;

            public Transform2D* Transform;
            public PhysicsCollider2D* PhysicsCollider;
            public HurtboxInstance* HurtboxInstance;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            if (f.Unsafe.TryGetPointer(filter.HurtboxInstance->Owner, out CustomAnimator* customAnimator))
            {
                if (f.Unsafe.TryGetPointer(filter.HurtboxInstance->Owner, out Transform2D* parentTransform))
                {
                    FPVector3 offset = CustomAnimator.GetFrame(f, customAnimator).hurtboxPositions[filter.HurtboxInstance->Index];

                    filter.Transform->Position = parentTransform->Position + offset.XY;
                }
            }

            ColorRGBA color = new() { R = 0, G = 0, B = 0, A = 128 };

            if (filter.HurtboxInstance->Settings.CanBeInterrupted)
                color.R = 255;
            if (filter.HurtboxInstance->Settings.CanBeKnockedBack)
                color.G = 255;
            if (filter.HurtboxInstance->Settings.CanBeDamaged)
                color.B = 255;

            Draw.Circle(filter.Transform->Position, filter.PhysicsCollider->Shape.BroadRadius);
        }
    }
}
