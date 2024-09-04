using Photon.Deterministic;
using System.Diagnostics;
using System;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class KnockbackState : PlayerState
    {
        public FP MaximumYVelocity;
        public int MaxInfluence;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.Shakeable->Time <= 0 &&
                   DoesStateTypeMatch(stateMachine, ref filter) && !filter.CharacterController->CurrentKnockback.Equals(default(KnockbackInfo)) && filter.CharacterController->HitStunTime > 0;
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            if (filter.CharacterController->OldKnockback.Direction.X > 0 && filter.CharacterController->GetNearbyCollider(Colliders.RightWall) ||
                filter.CharacterController->OldKnockback.Direction.X < 0 && filter.CharacterController->GetNearbyCollider(Colliders.LeftWall))
            {
                filter.CharacterController->CurrentKnockback.Direction.X *= -1;
            }

            PreviewKnockback(filter.CharacterController->OldKnockback.Direction, filter.CharacterController->OriginalPosition);

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "KnockbackY", filter.PhysicsBody->Velocity.Normalized.Y);
            --filter.CharacterController->HitStunTime;
        }

        protected override FP GetMovementInfluence(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => FPMath.Clamp(filter.CharacterController->StateTime / MaxInfluence, FP._0, FP._1);

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.CharacterController->HitStunTime <= 0 || filter.CharacterController->GetNearbyCollider(Colliders.Ground);
        }

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);

            filter.CharacterController->CurrentKnockback = default;
            filter.CharacterController->HitStunTime = 0;
        }

        [Conditional("DEBUG")]
        private void PreviewKnockback(FPVector2 amount, FPVector2 offset)
        {
            var lineList = CalculateArcPositions(20, amount, offset);

            for (int i = 0; i < lineList.Length - 1; ++i)
            {
                Draw.Line(lineList[i], lineList[i + 1]);
            }
        }

        private ReadOnlySpan<FPVector3> CalculateArcPositions(int resolution, FPVector2 amount, FPVector2 offset)
        {
            FPVector3[] positions = new FPVector3[resolution];

            for (int i = 0; i < resolution; ++i)
            {
                FP t = (FP)i / resolution;
                positions[i] = (CalculateArcPoint(t, 20, 1, amount) + offset).XYO;
            }

            return positions;
        }

        private FPVector2 CalculateArcPoint(FP t, FP gravity, FP scalar, FPVector2 amount)
        {
            amount.X += FP._1 / 10000;
            FP angle = FPMath.Atan2(amount.Y, amount.X);

            FP x = t * amount.X;
            FP y = x * FPMath.Tan(angle) - (gravity * x * x / (2 * amount.Magnitude * amount.Magnitude * FPMath.Cos(angle) * FPMath.Cos(angle)));

            return new FPVector2(x, y) * scalar;
        }
    }
}
