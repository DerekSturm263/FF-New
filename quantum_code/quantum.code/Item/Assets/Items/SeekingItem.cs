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
        public FP SeekTime;

        public override void OnStart(Frame f, EntityRef user, ref ItemSystem.Filter filter)
        {
            base.OnStart(f, user, ref filter);

            filter.ItemInstance->Target = PlayerStatsSystem.FindNearestOtherPlayer(f, user);
            filter.PhysicsBody->GravityScale = 0;
        }

        public override unsafe void OnUpdate(Frame f, EntityRef user, ref ItemSystem.Filter filter)
        {
            if (filter.ItemInstance->ActiveTime > SeekTime)
                return;

            if (f.Unsafe.TryGetPointer(filter.ItemInstance->Target, out Transform2D* targetTransform))
            {
                FPVector2 targetVelocity = ((targetTransform->Position + Offset) - filter.Transform->Position).Normalized * SpeedOverTime.Evaluate((FP)filter.ItemInstance->ActiveTime / (FP._1 * 60)) * SpeedMultiplier;

                filter.PhysicsBody->Velocity = FPVector2.Lerp(filter.PhysicsBody->Velocity, targetVelocity, f.DeltaTime * LerpTime);
            }
        }

        public override void OnExit(Frame f, EntityRef user, ref ItemSystem.Filter filter)
        {
            filter.ItemInstance->Target = EntityRef.None;
        }
    }
}
