using Photon.Deterministic;

namespace Quantum
{
    [System.Serializable]
    public unsafe partial class SeekingItem : UpdateableItem
    {
        public FPAnimationCurve SpeedOverTime;
        public FP SpeedMultiplier;
        public FP LerpTime;
        public FPVector2 Offset;

        public override void OnStart(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            itemInstance->Target = PlayerStatsSystem.FindNearestOtherPlayer(f, user);

            if (f.Unsafe.TryGetPointer(item, out PhysicsBody2D* physicsBody2D))
            {
                physicsBody2D->GravityScale = 0;
            }
        }

        public override unsafe void OnUpdate(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            if (f.Unsafe.TryGetPointer(item, out PhysicsBody2D* physicsBody) &&
                f.Unsafe.TryGetPointer(item, out Transform2D* transform) &&
                f.Unsafe.TryGetPointer(itemInstance->Target, out Transform2D* targetTransform))
            {
                FPVector2 targetVelocity = ((targetTransform->Position + Offset) - transform->Position).Normalized * SpeedOverTime.Evaluate(itemInstance->ActiveTime) * SpeedMultiplier;

                physicsBody->Velocity = FPVector2.Lerp(physicsBody->Velocity, targetVelocity, f.DeltaTime * LerpTime);
                transform->Rotation = FPMath.Atan2(physicsBody->Velocity.Y, physicsBody->Velocity.X);
            }
        }

        public override void OnExit(Frame f, EntityRef user, EntityRef item, ItemInstance* itemInstance)
        {
            itemInstance->Target = EntityRef.None;
        }
    }
}
