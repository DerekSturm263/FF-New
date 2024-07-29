using System.Diagnostics;

namespace Quantum
{
    public unsafe class HurtboxSystem : SystemMainThreadFilter<HurtboxSystem.Filter>, ISignalOnComponentAdded<HurtboxInstance>
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
            DrawHurtbox(filter.Transform, filter.PhysicsCollider, filter.HurtboxInstance);
        }

        public void OnAdded(Frame f, EntityRef entity, HurtboxInstance* component)
        {
            component->Owner = entity;
        }

        [Conditional("DEBUG")]
        public static void DrawHurtbox(Transform2D* transform, PhysicsCollider2D* physicsCollider, HurtboxInstance* hurtboxInstance)
        {
            ColorRGBA color = new() { R = 0, G = 0, B = 0, A = 128 };

            if (hurtboxInstance->Settings.CanBeInterrupted)
                color.R = 255;
            if (hurtboxInstance->Settings.CanBeKnockedBack)
                color.G = 255;
            if (hurtboxInstance->Settings.CanBeDamaged)
                color.B = 255;

            switch (physicsCollider->Shape.Type)
            {
                case Shape2DType.Circle:
                    Draw.Circle(transform->Position + physicsCollider->Shape.Centroid, physicsCollider->Shape.Circle.Radius, color);
                    break;

                case Shape2DType.Box:
                    Draw.Rectangle(transform->Position + physicsCollider->Shape.Centroid, physicsCollider->Shape.Box.Extents * 2, 0, color);
                    break;
            }
        }
    }
}
