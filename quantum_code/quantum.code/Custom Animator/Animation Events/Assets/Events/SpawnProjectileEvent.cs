
namespace Quantum
{
    [System.Serializable]
    public sealed unsafe partial class SpawnProjectileEvent : FrameEvent
    {
        public ProjectileSettings Settings;

        public override void Begin(Frame f, EntityRef entity, int frame)
        {
            Log.Debug("Spawning projectile!");

            EntityPrototype projectilePrototype = f.FindAsset<EntityPrototype>(Settings.Prototype.Id);
            EntityRef projectileEntity = f.Create(projectilePrototype);

            if (f.Unsafe.TryGetPointer(projectileEntity, out HitboxInstance* hitbox))
            {
                hitbox->Owner = entity;
            }

            if (f.Unsafe.TryGetPointer(projectileEntity, out PhysicsBody2D* physicsBody))
            {
                physicsBody->Velocity = Settings.Angle;
            }

            if (f.Unsafe.TryGetPointer(projectileEntity, out Transform2D* transform))
            {
                if (f.Unsafe.TryGetPointer(entity, out Transform2D* parentTransform))
                {
                    transform->Position = parentTransform->Position + Settings.Offset;
                }
            }
        }
    }
}
