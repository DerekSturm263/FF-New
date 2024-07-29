namespace Quantum
{
    public unsafe class ProjectileHelper
    {
        public static void SpawnProjectile(Frame f, ProjectileSettings settings, EntityRef user)
        {
            EntityPrototype projectilePrototype = f.FindAsset<EntityPrototype>(settings.Prototype.Id);
            EntityRef projectileEntity = f.Create(projectilePrototype);

            if (f.Unsafe.TryGetPointer(projectileEntity, out HitboxInstance* hitbox))
            {
                hitbox->Owner = user;
            }

            if (f.Unsafe.TryGetPointer(projectileEntity, out PhysicsBody2D* physicsBody))
            {
                physicsBody->Velocity = settings.Velocity;
            }

            if (f.Unsafe.TryGetPointer(projectileEntity, out Transform2D* transform) &&
                f.Unsafe.TryGetPointer(user, out Transform2D* parentTransform))
            {
                transform->Position = parentTransform->Position + settings.Offset;
            }
        }
    }
}
