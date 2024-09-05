using Photon.Deterministic;
using System.Diagnostics;
using System;
using Quantum.Inspector;

namespace Quantum
{
    [System.Serializable]
    public unsafe sealed class KnockbackState : PlayerState
    {
        [Header("State-Specific Values")]

        public int MaxInfluence;
        public FP InfluenceMultiplier;
        public FP DecelerationSpeed;

        public FP WallVelocityAbsorption;
        public FP MinimumVelocity;
        public int MinStateTimeToGrounded;

        protected override bool CanEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return DoesStateTypeMatch(stateMachine, ref filter) && filter.CharacterController->StartKnockback;
        }

        public override void BeginEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.BeginEnter(f, stateMachine, ref filter, input, settings, previousState);
         
            filter.CharacterController->StartKnockback = false;
        }

        public override void FinishEnter(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState previousState)
        {
            base.FinishEnter(f, stateMachine, ref filter, input, settings, previousState);

            filter.PhysicsBody->Velocity.Y = filter.CharacterController->CurrentKnockback.Direction.Y;
        }

        public override void Update(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            base.Update(f, stateMachine, ref filter, input, settings);

            if (filter.PhysicsBody->Velocity.X > 0 && filter.CharacterController->GetNearbyCollider(Colliders.RightWall) ||
                filter.PhysicsBody->Velocity.X < 0 && filter.CharacterController->GetNearbyCollider(Colliders.LeftWall))
            {
                filter.CharacterController->CurrentKnockback.Direction.X *= -WallVelocityAbsorption;

                if (FPMath.Abs(filter.CharacterController->CurrentKnockback.Direction.X) < MinimumVelocity)
                    filter.CharacterController->CurrentKnockback.Length = 1;
            }

            if (filter.CharacterController->CurrentKnockback.Direction.Y < 0 && filter.CharacterController->GetNearbyCollider(Colliders.Ground))
            {
                filter.CharacterController->CurrentKnockback.Direction.Y *= -WallVelocityAbsorption;
            }

            filter.CharacterController->CurrentKnockback.Direction.X = FPMath.Lerp(filter.CharacterController->CurrentKnockback.Direction.X, 0, f.DeltaTime * DecelerationSpeed);
            --filter.CharacterController->CurrentKnockback.Length;

            CustomAnimator.SetFixedPoint(f, filter.CustomAnimator, "KnockbackY", filter.PhysicsBody->Velocity.Normalized.Y);

            PreviewKnockback(filter.CharacterController->OldKnockback.Direction, filter.CharacterController->OriginalPosition);
        }

        protected override FP GetMovementInfluence(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings) => FPMath.Clamp(filter.CharacterController->StateTime / MaxInfluence, FP._0, FP._1) * InfluenceMultiplier;

        protected override bool CanExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings)
        {
            return filter.CharacterController->CurrentKnockback.Length <= 0 || (filter.CharacterController->GetNearbyCollider(Colliders.Ground) && filter.CharacterController->StateTime > MinStateTimeToGrounded);
        }

        public override void BeginExit(Frame f, PlayerStateMachine stateMachine, ref CharacterControllerSystem.Filter filter, Input input, MovementSettings settings, AssetRefPlayerState nextState)
        {
            filter.CharacterController->CurrentKnockback = default;

            base.BeginExit(f, stateMachine, ref filter, input, settings, nextState);
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
