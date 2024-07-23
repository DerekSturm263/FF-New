using Photon.Deterministic;
using System.Diagnostics;

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
            if (f.Unsafe.TryGetPointer(filter.HurtboxInstance->Owner, out CustomAnimator* customAnimator) &&
                f.Unsafe.TryGetPointer(filter.HurtboxInstance->Owner, out Transform2D* parentTransform))
            {
                FPVector3 offset = CustomAnimator.GetFrame(f, customAnimator).hurtboxPositions[filter.HurtboxInstance->Index];
                if (f.Unsafe.TryGetPointer(filter.HurtboxInstance->Owner, out CharacterController* characterController) && characterController->MovementDirection < 0)
                {
                    offset.X *= -1;
                }

                filter.Transform->Position = parentTransform->Position + offset.XY;
            }

            DrawHurtbox(filter.Transform, filter.PhysicsCollider, filter.HurtboxInstance);
        }

        [Conditional("DEBUG")]
        private void DrawHurtbox(Transform2D* transform, PhysicsCollider2D* physicsCollider, HurtboxInstance* hurtboxInstance)
        {
            ColorRGBA color = new() { R = 0, G = 0, B = 0, A = 128 };

            if (hurtboxInstance->Settings.CanBeInterrupted)
                color.R = 255;
            if (hurtboxInstance->Settings.CanBeKnockedBack)
                color.G = 255;
            if (hurtboxInstance->Settings.CanBeDamaged)
                color.B = 255;

            Draw.Circle(transform->Position, physicsCollider->Shape.BroadRadius);
        }
    }
}
