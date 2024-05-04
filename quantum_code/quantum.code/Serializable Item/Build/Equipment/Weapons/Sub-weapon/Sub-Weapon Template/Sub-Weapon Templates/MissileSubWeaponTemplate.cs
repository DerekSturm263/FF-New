using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class MissileSubWeaponTemplate : ExplodingSubWeaponTemplate
    {
        public FP Speed;
        public FP LerpTime;

        public override void OnUpdate(Frame f, EntityRef user, EntityRef target, EntityRef subWeaponInstance)
        {
            if (f.Unsafe.TryGetPointer(subWeaponInstance, out PhysicsBody2D* physicsBody))
            {
                if (f.Unsafe.TryGetPointer(subWeaponInstance, out Transform2D* transform))
                {
                    if (f.Unsafe.TryGetPointer(target, out Transform2D* targetTransform))
                    {
                        physicsBody->Velocity = FPVector2.Lerp(physicsBody->Velocity, (targetTransform->Position - transform->Position).Normalized * Speed, f.DeltaTime * LerpTime);
                    }
                }
            }
        }
    }
}
